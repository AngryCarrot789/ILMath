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

    /// <summary>
    /// Compiles the syntax tree into a function.
    /// </summary>
    /// <param name="name">The name of the function.</param>
    /// <param name="tree">The syntax tree to compile.</param>
    /// <returns>The evaluator.</returns>
    public Evaluator<T> Compile(string name, INode tree) {
        return this.CompileSyntaxTree(tree);
    }

    private Evaluator<T> CompileSyntaxTree(INode rootNode) {
        ParameterExpression parameter = Expression.Parameter(typeof(IEvaluationContext<T>));
        Expression compiledExpressionTree = this.CompileNode(rootNode, parameter);
        return Expression.Lambda<Evaluator<T>>(compiledExpressionTree, parameter).Compile();
    }

    private Expression CompileNode(INode node, ParameterExpression parameter) {
        return node switch {
            OperatorNode expressionNode => this.CompileOperatorNode(expressionNode, parameter),
            NumberNode<T> numberNode => CompileNumberNode(numberNode, parameter),
            UnaryNode unaryNode => this.CompileUnaryNode(unaryNode, parameter),
            VariableNode variableNode => CompileVariableNode(variableNode, parameter),
            FunctionNode functionNode => this.CompileFunctionNode(functionNode, parameter),
            _ => throw new CompilerException($"Unknown node type: {node.GetType()}")
        };
    }

    private Expression CompileOperatorNode(OperatorNode operatorNode, ParameterExpression parameter) {
        INode left = operatorNode.Left;
        INode right = operatorNode.Right;
        OperatorType @operator = operatorNode.Operator;
        Expression compiledLeft = this.CompileNode(left, parameter);
        Expression compiledRight = this.CompileNode(right, parameter);
        switch (@operator) {
            case OperatorType.Plus:           return Expression.Add(compiledLeft, compiledRight);
            case OperatorType.Minus:          return Expression.Subtract(compiledLeft, compiledRight);
            case OperatorType.Multiplication: return Expression.Multiply(compiledLeft, compiledRight);
            case OperatorType.Division:       return Expression.Divide(compiledLeft, compiledRight);
            case OperatorType.Modulo:         return Expression.Modulo(compiledLeft, compiledRight);
        }

        if (!Util<T>.IsFP) {
            switch (@operator) {
                case OperatorType.Xor:    return Expression.ExclusiveOr(compiledLeft, compiledRight);
                case OperatorType.LShift: return Expression.LeftShift(compiledLeft, compiledRight, Util<T>.LeftShift);
                case OperatorType.RShift: return Expression.RightShift(compiledLeft, compiledRight, Util<T>.RightShift);
                case OperatorType.And:    return Expression.And(compiledLeft, compiledRight);
                case OperatorType.Or:     return Expression.Or(compiledLeft, compiledRight);
            }
        }

        throw new CompilerException($"Unknown operator: {@operator}");
    }

    private static Expression CompileNumberNode(NumberNode<T> numberNode, ParameterExpression _) {
        return Expression.Constant(numberNode.Value);
    }

    private Expression CompileUnaryNode(UnaryNode unaryNode, ParameterExpression parameter) {
        Expression compiledChild = this.CompileNode(unaryNode.Child, parameter);
        switch (unaryNode.Operator) {
            case OperatorType.Plus:           return compiledChild;
            case OperatorType.Minus:          return Expression.Negate(compiledChild, Util<T>.Negate);
            case OperatorType.OnesComplement: return Expression.OnesComplement(compiledChild);
            default:                          throw new CompilerException($"Unknown unary operator: {unaryNode.Operator}");
        }
    }

    private static Expression CompileVariableNode(VariableNode variableNode, ParameterExpression parameter) {
        return Expression.Call(parameter, MethodInfo_GetVariable, Expression.Constant(variableNode.Identifier));
    }

    private Expression CompileFunctionNode(FunctionNode functionNode, ParameterExpression parameter) {
        IReadOnlyList<INode> parameters = functionNode.Parameters;
        IEnumerable<Expression> compiledParameters = parameters.Select(x => this.CompileNode(x, parameter));
        return Expression.Call(MethodInfo_CallMethod, parameter, Expression.Constant(functionNode.Identifier), Expression.NewArrayInit(typeof(T), compiledParameters));
    }

    private static T CallMethod(IEvaluationContext<T> context, string identifier, T[] parameters) {
        return context.CallFunction(identifier, parameters);
    }
}