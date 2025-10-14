using System.Numerics;
using ILMath.Exception;

namespace ILMath;

public delegate T MathFunction<T>(Span<T> parameters) where T : unmanaged, INumber<T>;

/// <summary>
/// Default implementation of <see cref="IEvaluationContext{T}"/>.
/// </summary>
public abstract class EvaluationContext<T> : IEvaluationContext<T> where T : unmanaged, INumber<T> {
    private readonly Dictionary<string, T> variables = new();
    private readonly Dictionary<string, MathFunction<T>> functions = new();

    protected EvaluationContext() {
    }

    /// <summary>
    /// Registers default variables and functions to this <see cref="EvaluationContext{T}"/>.
    /// </summary>
    protected internal virtual void RegisterBuiltIns() {
        this.RegisterFunction("mod", static p => p[0] % p[1]);
        this.RegisterFunction("abs", static p => T.Abs(p[0]));
        this.RegisterFunction("min", static p => T.Min(p[0], p[1]));
        this.RegisterFunction("max", static p => T.Max(p[0], p[1]));
        this.RegisterFunction("clamp", static p => T.Clamp(p[0], p[1], p[2]));
        this.RegisterFunction("lerp", static p => (p[2] - p[1]) * p[0] + p[1]);
        this.RegisterFunction("inverseLerp", static p => (p[0] - p[1]) / (p[2] - p[1]));
    }

    /// <summary>
    /// Registers a variable to this <see cref="EvaluationContext{T}"/>.
    /// </summary>
    /// <param name="identifier">The variable's identifier.</param>
    /// <param name="value">The variable's value.</param>
    public void RegisterVariable(string identifier, T value) {
        this.variables.Add(identifier, value);
    }

    /// <summary>
    /// Registers a function to this <see cref="EvaluationContext{T}"/>.
    /// </summary>
    /// <param name="identifier">The function's identifier.</param>
    /// <param name="function">The function.</param>
    public void RegisterFunction(string identifier, MathFunction<T> function) {
        this.functions.Add(identifier, function);
    }

    public T GetVariable(string identifier) {
        if (this.variables.TryGetValue(identifier, out T value))
            return value;
        throw new EvaluationException($"Unknown variable: {identifier}");
    }

    public T CallFunction(string identifier, Span<T> parameters) {
        if (this.functions.TryGetValue(identifier, out MathFunction<T>? function))
            return function(parameters);
        throw new EvaluationException($"Unknown function: {identifier}");
    }
}

public static class EvaluationContexts {
    public static EvaluationContext<double> CreateForDouble() {
        EvaluationContextDouble context = new EvaluationContextDouble();
        context.RegisterBuiltIns();
        return context;
    }

    public static EvaluationContext<float> CreateForFloat() {
        EvaluationContextFloat context = new EvaluationContextFloat();
        context.RegisterBuiltIns();
        return context;
    }

    public static EvaluationContext<T> CreateForInteger<T>() where T : unmanaged, INumber<T>, IBinaryNumber<T> {
        EvaluationContextBinaryNumber<T> context = new EvaluationContextBinaryNumber<T>();
        context.RegisterBuiltIns();
        return context;
    }
}

public sealed class EvaluationContextDouble : EvaluationContext<double> {
    internal EvaluationContextDouble() {
    }

    protected internal override void RegisterBuiltIns() {
        base.RegisterBuiltIns();

        this.RegisterVariable("pi", Math.PI);
        this.RegisterVariable("e", Math.E);
        this.RegisterVariable("tau", Math.PI * 2.0);
        this.RegisterVariable("phi", (1.0 + Math.Sqrt(5.0)) / 2.0);
        this.RegisterVariable("inf", double.PositiveInfinity);
        this.RegisterVariable("nan", double.NaN);
        this.RegisterVariable("degToRad", Math.PI / 180.0);
        this.RegisterVariable("radToDeg", 180.0 / Math.PI);

        this.RegisterFunction("sin", static p => Math.Sin(p[0]));
        this.RegisterFunction("cos", static p => Math.Cos(p[0]));
        this.RegisterFunction("tan", static p => Math.Tan(p[0]));
        this.RegisterFunction("asin", static p => Math.Asin(p[0]));
        this.RegisterFunction("acos", static p => Math.Acos(p[0]));
        this.RegisterFunction("atan", static p => Math.Atan(p[0]));
        this.RegisterFunction("atan2", static p => Math.Atan2(p[0], p[1]));
        this.RegisterFunction("sinh", static p => Math.Sinh(p[0]));
        this.RegisterFunction("cosh", static p => Math.Cosh(p[0]));
        this.RegisterFunction("tanh", static p => Math.Tanh(p[0]));
        this.RegisterFunction("sqrt", static p => Math.Sqrt(p[0]));
        this.RegisterFunction("cbrt", static p => Math.Cbrt(p[0]));
        this.RegisterFunction("root", static p => Math.Pow(p[0], 1.0 / p[1]));
        this.RegisterFunction("exp", static p => Math.Exp(p[0]));
        this.RegisterFunction("log", static p => Math.Log(p[0]));
        this.RegisterFunction("log10", static p => Math.Log10(p[0]));
        this.RegisterFunction("log2", static p => Math.Log2(p[0]));
        this.RegisterFunction("logn", static p => Math.Log(p[0], p[1]));
        this.RegisterFunction("pow", static p => Math.Pow(p[0], p[1]));
        this.RegisterFunction("floor", static p => Math.Floor(p[0]));
        this.RegisterFunction("ceil", static p => Math.Ceiling(p[0]));
        this.RegisterFunction("round", static p => Math.Round(p[0]));
    }
}

public sealed class EvaluationContextFloat : EvaluationContext<float> {
    internal EvaluationContextFloat() {
    }

    protected internal override void RegisterBuiltIns() {
        base.RegisterBuiltIns();

        this.RegisterVariable("pi", (float) (Math.PI));
        this.RegisterVariable("e", (float) (Math.E));
        this.RegisterVariable("tau", (float) (Math.PI * 2.0));
        this.RegisterVariable("phi", (float) ((1.0 + Math.Sqrt(5.0)) / 2.0));
        this.RegisterVariable("inf", (float) (double.PositiveInfinity));
        this.RegisterVariable("nan", (float) (double.NaN));
        this.RegisterVariable("degToRad", (float) (Math.PI / 180.0));
        this.RegisterVariable("radToDeg", (float) (180.0 / Math.PI));

        this.RegisterFunction("sin", static p => (float) Math.Sin(p[0]));
        this.RegisterFunction("cos", static p => (float) Math.Cos(p[0]));
        this.RegisterFunction("tan", static p => (float) Math.Tan(p[0]));
        this.RegisterFunction("asin", static p => (float) Math.Asin(p[0]));
        this.RegisterFunction("acos", static p => (float) Math.Acos(p[0]));
        this.RegisterFunction("atan", static p => (float) Math.Atan(p[0]));
        this.RegisterFunction("atan2", static p => (float) Math.Atan2(p[0], p[1]));
        this.RegisterFunction("sinh", static p => (float) Math.Sinh(p[0]));
        this.RegisterFunction("cosh", static p => (float) Math.Cosh(p[0]));
        this.RegisterFunction("tanh", static p => (float) Math.Tanh(p[0]));
        this.RegisterFunction("sqrt", static p => (float) Math.Sqrt(p[0]));
        this.RegisterFunction("cbrt", static p => (float) Math.Cbrt(p[0]));
        this.RegisterFunction("root", static p => (float) Math.Pow(p[0], 1.0 / p[1]));
        this.RegisterFunction("exp", static p => (float) Math.Exp(p[0]));
        this.RegisterFunction("log", static p => (float) Math.Log(p[0]));
        this.RegisterFunction("log10", static p => (float) Math.Log10(p[0]));
        this.RegisterFunction("log2", static p => (float) Math.Log2(p[0]));
        this.RegisterFunction("logn", static p => (float) Math.Log(p[0], p[1]));
        this.RegisterFunction("pow", static p => (float) Math.Pow(p[0], p[1]));
        this.RegisterFunction("floor", static p => (float) Math.Floor(p[0]));
        this.RegisterFunction("ceil", static p => (float) Math.Ceiling(p[0]));
        this.RegisterFunction("round", static p => (float) Math.Round(p[0]));
    }
}

public sealed class EvaluationContextBinaryNumber<T> : EvaluationContext<T> where T : unmanaged, INumber<T>, IBinaryNumber<T> {
    internal EvaluationContextBinaryNumber() {
    }

    protected internal override void RegisterBuiltIns() {
        base.RegisterBuiltIns();
        this.RegisterFunction("log2", static p => T.Log2(p[0]));
    }
}