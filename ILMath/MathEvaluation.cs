using System.Numerics;
using ILMath.Compiler;
using ILMath.SyntaxTree;

namespace ILMath;

/// <summary>
/// Helper class for math evaluation.
/// </summary>
public static class MathEvaluation {
    /// <summary>
    /// Compiles an expression to a function.
    /// </summary>
    /// <param name="functionName">The function name.</param>
    /// <param name="expression">The math expression.</param>
    /// <param name="method">The compilation method.</param>
    /// <returns>The evaluator.</returns>
    public static Evaluator<T> CompileExpression<T>(string functionName, string expression, CompilationMethod method = CompilationMethod.IntermediateLanguage) where T : unmanaged, INumber<T> {
        Lexer lexer = new Lexer(expression);
        Parser<T> parser = new Parser<T>(lexer);
        INode node = parser.Parse();
        ICompiler<T> compiler = CreateCompiler<T>(method);
        return compiler.Compile(functionName, node);
    }

    /// <summary>
    /// Creates a compiler based on the compilation method.
    /// </summary>
    /// <param name="method">The compilation method.</param>
    /// <returns>The created compiler.</returns>
    public static ICompiler<T> CreateCompiler<T>(CompilationMethod method) where T : unmanaged, INumber<T> {
        switch (method) {
            case CompilationMethod.IntermediateLanguage: return IlCompiler<T>.Instance;
            case CompilationMethod.Functional:           return FunctionalCompiler<T>.Instance;
            case CompilationMethod.ExpressionTree:       return ExpressionTreeCompiler<T>.Instance;
            default:                                     throw new ArgumentException($"Unknown compilation method: {method}");
        }
    }
}