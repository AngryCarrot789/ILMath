using System.Numerics;
using System.Runtime.CompilerServices;
using ILMath.Exception;
using ILMath.SyntaxTree;

namespace ILMath.Compiler;

/// <summary>
/// Compiles the expression using functional methods.
/// </summary>
public class FunctionalCompiler<T> : ICompiler<T> where T : unmanaged, INumber<T> {
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
        Evaluator<T> compiledRoot = CompileNode(rootNode);
        return context => compiledRoot(context);
    }

    private static Evaluator<T> CompileNode(INode node) {
        return node switch {
            OperatorNode expressionNode => CompileOperatorNode(expressionNode),
            LiteralNode<T> numberNode => CompileNumberNode(numberNode),
            UnaryNode unaryNode => CompileUnaryNode(unaryNode),
            VariableNode variableNode => CompileVariableNode(variableNode),
            FunctionNode functionNode => CompileFunctionNode(functionNode),
            _ => throw new CompilerException($"Unknown node type: {node.GetType()}")
        };
    }

    private static Evaluator<T> CompileOperatorNode(OperatorNode operatorNode) {
        INode left = operatorNode.Left;
        INode right = operatorNode.Right;
        OperatorType @operator = operatorNode.Operator;
        Evaluator<T> compiledLeft = CompileNode(left);
        Evaluator<T> compiledRight = CompileNode(right);
        switch (@operator) {
            case OperatorType.Plus:           return context => compiledLeft(context) + compiledRight(context);
            case OperatorType.Minus:          return context => compiledLeft(context) - compiledRight(context);
            case OperatorType.Multiplication: return context => compiledLeft(context) * compiledRight(context);
            case OperatorType.Division:       return context => compiledLeft(context) / compiledRight(context);
            case OperatorType.Modulo:         return context => compiledLeft(context) % compiledRight(context);
        }

        if (!Util<T>.IsFP) {
            switch (@operator) {
                case OperatorType.Xor:    return context => Xor(compiledLeft(context), compiledRight(context));
                case OperatorType.LShift: return context => LShift(compiledLeft(context), compiledRight(context));
                case OperatorType.RShift: return context => RShift(compiledLeft(context), compiledRight(context));
                case OperatorType.And:    return context => And(compiledLeft(context), compiledRight(context));
                case OperatorType.Or:     return context => Or(compiledLeft(context), compiledRight(context));
            }
        }

        throw new CompilerException($"Unknown operator: {@operator}");
    }

    private static Evaluator<T> CompileNumberNode(LiteralNode<T> literalNode) {
        T value = literalNode.Value;
        return _ => value;
    }

    private static Evaluator<T> CompileUnaryNode(UnaryNode unaryNode) {
        Evaluator<T> compiledChild = CompileNode(unaryNode.Child);
        switch (unaryNode.Operator) {
            case OperatorType.Plus:           return context => compiledChild(context);
            case OperatorType.Minus:          return context => -compiledChild(context);
            case OperatorType.OnesComplement: return context => OnesComplement(compiledChild(context));
            case OperatorType.BoolNot:        return context => Not(compiledChild(context));
        }

        throw new CompilerException($"Unknown unary operator: {unaryNode.Operator}");
    }

    private static Evaluator<T> CompileVariableNode(VariableNode variableNode) {
        string identifier = variableNode.Identifier;
        return context => context.GetVariable(identifier);
    }

    private static Evaluator<T> CompileFunctionNode(FunctionNode functionNode) {
        IReadOnlyList<INode> parameters = functionNode.Parameters;
        Evaluator<T>[] compiledParameters = parameters.Select(CompileNode).ToArray();
        return context => {
            Span<T> values = stackalloc T[compiledParameters.Length];
            for (int i = 0; i < compiledParameters.Length; i++)
                values[i] = compiledParameters[i](context);
            return context.CallFunction(functionNode.Identifier, values);
        };
    }

    private static T OnesComplement(T input) {
        if (typeof(T) == typeof(int))
            return Operate1<int>(input, static x => ~x);
        if (typeof(T) == typeof(uint))
            return Operate1<uint>(input, static x => ~x);
        if (typeof(T) == typeof(long))
            return Operate1<long>(input, static x => ~x);
        if (typeof(T) == typeof(ulong))
            return Operate1<ulong>(input, static x => ~x);
        throw new EvaluationException($"Unknown type: {typeof(T)}");
    }
    
    private static T Not(T input) {
        if (typeof(T) == typeof(int))
            return Operate1<int>(input, static x => x != 0 ? 0 : 1);
        if (typeof(T) == typeof(uint))
            return Operate1<uint>(input, static x => x != 0 ? 0U : 1U);
        if (typeof(T) == typeof(long))
            return Operate1<long>(input, static x => x != 0 ? 0U : 1U);
        if (typeof(T) == typeof(ulong))
            return Operate1<ulong>(input, static x => x != 0 ? 0U : 1U);
        throw new EvaluationException($"Unknown type: {typeof(T)}");
    }

    private static T Xor(T a, T b) {
        if (typeof(T) == typeof(int))
            return Operate2<int>(a, b, static (x, y) => (x ^ y));
        if (typeof(T) == typeof(uint))
            return Operate2<uint>(a, b, static (x, y) => (x ^ y));
        if (typeof(T) == typeof(long))
            return Operate2<long>(a, b, static (x, y) => (x ^ y));
        if (typeof(T) == typeof(ulong))
            return Operate2<ulong>(a, b, static (x, y) => (x ^ y));
        throw new EvaluationException($"Unknown type: {typeof(T)}");
    }

    private static T LShift(T a, T b) {
        if (typeof(T) == typeof(int))
            return Operate2<int>(a, b, static (x, y) => (x << y));
        if (typeof(T) == typeof(uint))
            return Operate2<uint>(a, b, static (x, y) => (x << (int) y));
        if (typeof(T) == typeof(long))
            return Operate2<long>(a, b, static (x, y) => (x << (int) y));
        if (typeof(T) == typeof(ulong))
            return Operate2<ulong>(a, b, static (x, y) => (x << (int) y));
        throw new EvaluationException($"Unknown type: {typeof(T)}");
    }

    private static T RShift(T a, T b) {
        if (typeof(T) == typeof(int))
            return Operate2<int>(a, b, static (x, y) => (x >> y));
        if (typeof(T) == typeof(uint))
            return Operate2<uint>(a, b, static (x, y) => (x >> (int) y));
        if (typeof(T) == typeof(long))
            return Operate2<long>(a, b, static (x, y) => (x >> (int) y));
        if (typeof(T) == typeof(ulong))
            return Operate2<ulong>(a, b, static (x, y) => (x >> (int) y));
        throw new EvaluationException($"Unknown type: {typeof(T)}");
    }

    private static T And(T a, T b) {
        if (typeof(T) == typeof(int))
            return Operate2<int>(a, b, static (x, y) => (x & y));
        if (typeof(T) == typeof(uint))
            return Operate2<uint>(a, b, static (x, y) => (x & y));
        if (typeof(T) == typeof(long))
            return Operate2<long>(a, b, static (x, y) => (x & y));
        if (typeof(T) == typeof(ulong))
            return Operate2<ulong>(a, b, static (x, y) => (x & y));
        throw new EvaluationException($"Unknown type: {typeof(T)}");
    }

    private static T Or(T a, T b) {
        if (typeof(T) == typeof(int))
            return Operate2<int>(a, b, static (x, y) => (x | y));
        if (typeof(T) == typeof(uint))
            return Operate2<uint>(a, b, static (x, y) => (x | y));
        if (typeof(T) == typeof(long))
            return Operate2<long>(a, b, static (x, y) => (x | y));
        if (typeof(T) == typeof(ulong))
            return Operate2<ulong>(a, b, static (x, y) => (x | y));
        throw new EvaluationException($"Unknown type: {typeof(T)}");
    }

    private static T Operate1<TTo>(T input, Func<TTo, TTo> operate) {
        TTo tIn = Unsafe.As<T, TTo>(ref input);
        TTo ret = operate(tIn);
        return Unsafe.As<TTo, T>(ref ret);
    }

    private static T Operate2<TTo>(T a, T b, Func<TTo, TTo, TTo> operate) {
        TTo tA = Unsafe.As<T, TTo>(ref a);
        TTo tB = Unsafe.As<T, TTo>(ref b);
        TTo ret = operate(tA, tB);
        return Unsafe.As<TTo, T>(ref ret);
    }
}