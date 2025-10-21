using System.Numerics;

namespace ILMath;

public delegate T EvaluationFunction<T>(Span<T> parameters) where T : unmanaged, INumber<T>;

/// <summary>
/// Defines a function within an <see cref="IEvaluationContext{T}"/>
/// </summary>
/// <param name="function">The function to invoke</param>
/// <param name="minParams">The minimum number of params</param>
/// <param name="maxParams">The maximum number of params</param>
/// <typeparam name="T">The evaluation value type</typeparam>
public readonly struct FunctionDeclaration<T>(EvaluationFunction<T> function, int minParams = -1, int maxParams = -1) where T : unmanaged, INumber<T> {
    /// <summary>
    /// Gets the math function
    /// </summary>
    public readonly EvaluationFunction<T> Function = function;

    /// <summary>
    /// Gets the minimum number of parameters allowed. Default is -1, meaning no limit
    /// </summary>
    public readonly int MinParams = minParams;

    /// <summary>
    /// Gets the maximum number of parameters allowed. Default is -1, meaning no limit
    /// </summary>
    public readonly int MaxParams = maxParams;
}