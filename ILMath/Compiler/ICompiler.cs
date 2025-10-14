using System.Numerics;
using ILMath.SyntaxTree;

namespace ILMath.Compiler;

/// <summary>
/// Common interface for compiler types.
/// </summary>
public interface ICompiler<T> where T : unmanaged, INumber<T> {
    /// <summary>
    /// Compiles the syntax tree into a function.
    /// </summary>
    /// <param name="name">The name of the function.</param>
    /// <param name="tree">The syntax tree to compile.</param>
    /// <returns>The evaluator.</returns>
    Evaluator<T> Compile(string name, INode tree);
}