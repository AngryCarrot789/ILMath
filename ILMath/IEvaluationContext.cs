using System.Numerics;
using ILMath.Exception;

namespace ILMath;

/// <summary>
/// Provides context for evaluating a compiled expression.
/// </summary>
public interface IEvaluationContext<T> where T : unmanaged, INumber<T> {
    /// <summary>
    /// Gets the value of a variable.
    /// </summary>
    /// <param name="identifier">The variable's identifier.</param>
    /// <returns>The variable's value.</returns>
    /// <exception cref="EvaluationException">No such variable</exception>
    T GetVariable(string identifier);

    /// <summary>
    /// Calls a function.
    /// </summary>
    /// <param name="identifier">The function's identifier.</param>
    /// <param name="parameters">The function's parameters.</param>
    /// <returns>The returned result of the function.</returns>
    /// <exception cref="EvaluationException">No such method</exception>
    T CallFunction(string identifier, Span<T> parameters);
}