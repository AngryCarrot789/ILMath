using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using ILMath.Exception;
using ILMath.SyntaxTree;

namespace ILMath.Compiler;

/// <summary>
/// Compiles the expression using expression trees.
/// </summary>
public class ExpressionTreeCompiler<T> : ICompiler<T> where T : unmanaged, INumber<T> {
    private static readonly MethodInfo MethodInfo_CallMethod = typeof(ExpressionTreeCompiler<T>).GetMethod(nameof(CallMethod), BindingFlags.Static | BindingFlags.NonPublic)!;
    private static readonly MethodInfo MethodInfo_GetVariable = typeof(IEvaluationContext<T>).GetMethod(nameof(IEvaluationContext<T>.GetVariable))!;
    internal static readonly ICompiler<T> Instance = new ExpressionTreeCompiler<T>();

    private static readonly Expression TrueExpression = Expression.Constant(Util<T>.BoolTrue);
    private static readonly Expression FalseExpression = Expression.Constant(Util<T>.BoolFalse);

    internal ExpressionTreeCompiler() {
    }

    /// <summary>
    /// Compiles the syntax tree into a function.
    /// </summary>
    /// <param name="name">The name of the function.</param>
    /// <param name="tree">The syntax tree to compile.</param>
    /// <returns>The evaluator.</returns>
    public Evaluator<T> Compile(string name, INode tree) {
        return CompileSyntaxTree(tree);
    }

    private static Evaluator<T> CompileSyntaxTree(INode rootNode) {
        ParameterExpression parameter = Expression.Parameter(typeof(IEvaluationContext<T>));
        Expression compiledExpressionTree = CompileNode(rootNode, parameter);
        return Expression.Lambda<Evaluator<T>>(compiledExpressionTree, parameter).Compile();
    }

    private static Expression CompileNode(INode node, ParameterExpression parameter) {
        return node switch {
            OperatorNode expressionNode => CompileOperatorNode(expressionNode, parameter),
            LiteralNode<T> numberNode => CompileNumberNode(numberNode, parameter),
            UnaryNode unaryNode => CompileUnaryNode(unaryNode, parameter),
            VariableNode variableNode => CompileVariableNode(variableNode, parameter),
            FunctionNode functionNode => CompileFunctionNode(functionNode, parameter),
            _ => throw new CompilerException($"Unknown node type: {node.GetType()}")
        };
    }

    private static Expression CompileOperatorNode(OperatorNode operatorNode, ParameterExpression parameter) {
        INode left = operatorNode.Left;
        INode right = operatorNode.Right;
        OperatorType @operator = operatorNode.Operator;
        Expression compiledLeft = CompileNode(left, parameter);
        Expression compiledRight = CompileNode(right, parameter);
        switch (@operator) {
            case OperatorType.Plus:           return Expression.Add(compiledLeft, compiledRight);
            case OperatorType.Minus:          return Expression.Subtract(compiledLeft, compiledRight);
            case OperatorType.Multiplication: return Expression.Multiply(compiledLeft, compiledRight);
            case OperatorType.Division:       return Expression.Divide(compiledLeft, compiledRight);
            case OperatorType.Modulo:         return Expression.Modulo(compiledLeft, compiledRight);

            case OperatorType.EqualTo:              return Expression.Condition(Expression.Equal(compiledLeft, compiledRight), TrueExpression, FalseExpression);
            case OperatorType.NotEqualTo:           return Expression.Condition(Expression.NotEqual(compiledLeft, compiledRight), TrueExpression, FalseExpression);
            case OperatorType.LessThan:             return Expression.Condition(Expression.LessThan(compiledLeft, compiledRight), TrueExpression, FalseExpression);
            case OperatorType.LessThanOrEqualTo:    return Expression.Condition(Expression.LessThanOrEqual(compiledLeft, compiledRight), TrueExpression, FalseExpression);
            case OperatorType.GreaterThan:          return Expression.Condition(Expression.GreaterThan(compiledLeft, compiledRight), TrueExpression, FalseExpression);
            case OperatorType.GreaterThanOrEqualTo: return Expression.Condition(Expression.GreaterThanOrEqual(compiledLeft, compiledRight), TrueExpression, FalseExpression);

            case OperatorType.ConditionalAnd:
                return Expression.Condition(
                    Expression.AndAlso(
                        Expression.NotEqual(compiledLeft, FalseExpression),
                        Expression.NotEqual(compiledRight, FalseExpression)),
                    TrueExpression,
                    FalseExpression
                );

            case OperatorType.ConditionalOr:
                return Expression.Condition(
                    Expression.OrElse(
                        Expression.NotEqual(compiledLeft, FalseExpression),
                        Expression.NotEqual(compiledRight, FalseExpression)),
                    TrueExpression,
                    FalseExpression
                );
        }

        if (!Util<T>.IsFP) {
            switch (@operator) {
                case OperatorType.Xor:    return Expression.ExclusiveOr(compiledLeft, compiledRight);
                case OperatorType.LShift: return Expression.LeftShift(compiledLeft, compiledRight, Util<T>.LeftShiftMethod);
                case OperatorType.RShift: return Expression.RightShift(compiledLeft, compiledRight, Util<T>.RightShiftMethod);
                case OperatorType.And:    return Expression.And(compiledLeft, compiledRight);
                case OperatorType.Or:     return Expression.Or(compiledLeft, compiledRight);
            }
        }

        throw new CompilerException($"Unexpected operator '{@operator.ToToken()}'");
    }

    private static Expression CompileNumberNode(LiteralNode<T> literalNode, ParameterExpression _) {
        return Expression.Constant(literalNode.Value);
    }

    private static Expression CompileUnaryNode(UnaryNode unaryNode, ParameterExpression parameter) {
        Expression compiledChild = CompileNode(unaryNode.Child, parameter);
        switch (unaryNode.Operator) {
            case OperatorType.Plus:           return compiledChild;
            case OperatorType.Minus:          return Expression.Negate(compiledChild, Util<T>.NegateMethod);
            case OperatorType.OnesComplement: return Expression.OnesComplement(compiledChild);
            case OperatorType.BoolNot:        return Expression.Condition(Expression.NotEqual(compiledChild, Expression.Constant(0, compiledChild.Type)), Expression.Constant(0, compiledChild.Type), Expression.Constant(1, compiledChild.Type));
            default:                          throw new CompilerException($"Unknown unary operator: {unaryNode.Operator}");
        }
    }

    private static Expression CompileVariableNode(VariableNode variableNode, ParameterExpression parameter) {
        return Expression.Call(parameter, MethodInfo_GetVariable, Expression.Constant(variableNode.Identifier));
    }

    private static Expression CompileFunctionNode(FunctionNode functionNode, ParameterExpression parameter) {
        IReadOnlyList<INode> parameters = functionNode.Parameters;
        IEnumerable<Expression> compiledParameters = parameters.Select(x => CompileNode(x, parameter));
        return Expression.Call(MethodInfo_CallMethod, parameter, Expression.Constant(functionNode.Identifier), Expression.NewArrayInit(typeof(T), compiledParameters));
    }

    private static T CallMethod(IEvaluationContext<T> context, string identifier, T[] parameters) {
        return context.CallFunction(identifier, parameters);
    }
}