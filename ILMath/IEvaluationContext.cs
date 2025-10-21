using System.Numerics;
using ILMath.Exception;

namespace ILMath;

/// <summary>
/// Provides context for evaluating a compiled expression.
/// </summary>
/// <typeparam name="T">The evaluation value type</typeparam>
public interface IEvaluationContext<T> where T : unmanaged, INumber<T> {
    /// <summary>
    /// Tries to get a variable with the given name
    /// </summary>
    /// <param name="identifier">The variable's identifier.</param>
    /// <param name="variable">The variable</param>
    /// <returns>True if the variable was found</returns>
    public bool TryGetVariable(string identifier, out T variable);
    
    /// <summary>
    /// Tries to get a function with the given name
    /// </summary>
    /// <param name="identifier">The function's identifier.</param>
    /// <param name="declaration">The function declaration</param>
    /// <returns>True if the function was found</returns>
    public bool TryGetFunction(string identifier, out FunctionDeclaration<T> declaration);
    
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