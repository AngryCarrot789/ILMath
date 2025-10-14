using System.Numerics;

namespace ILMath;

/// <summary>
/// Represents a function that is compiled to evaluate an expression.
/// </summary>
public delegate T Evaluator<T>(IEvaluationContext<T> context) where T : unmanaged, INumber<T>;