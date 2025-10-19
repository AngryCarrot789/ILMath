using System.Numerics;
using ILMath.Exception;

namespace ILMath;

public delegate T MathFunction<T>(Span<T> parameters) where T : unmanaged, INumber<T>;

/// <summary>
/// Default implementation of <see cref="IEvaluationContext{T}"/>.
/// </summary>
public abstract class EvaluationContext<T> : IEvaluationContext<T> where T : unmanaged, INumber<T> {
    public Dictionary<string, MathFunction<T>> Functions { get; } = new();
    public Dictionary<string, T> Variables { get; } = new();

    protected EvaluationContext() {
    }

    /// <summary>
    /// Registers default variables and functions to this <see cref="EvaluationContext{T}"/>.
    /// </summary>
    protected internal virtual void RegisterBuiltIns() {
        this.Functions["mod"] = static p => p[0] % p[1];
        this.Functions["abs"] = static p => T.Abs(p[0]);
        this.Functions["min"] = static p => Min(p);
        this.Functions["max"] = static p => Max(p);
        this.Functions["sum"] = static p => Sum(p);
        this.Functions["mean"] = static p => Sum(p) / T.CreateChecked(p.Length);
        this.Functions["average"] = static p => Sum(p) / T.CreateChecked(p.Length);
        this.Functions["range"] = static p => {
            if (p.Length < 1)
                return default;
            if (p.Length == 1)
                return p[0];
            T min = p[0], max = min;
            for (int i = 1; i < p.Length; i++) {
                T val = p[i];
                if (val < min)
                    min = val;
                if (val > max)
                    max = val;
            }

            return max - min;
        };
        this.Functions["clamp"] = static p => T.Clamp(p[0], p[1], p[2]);
        this.Functions["mode"] = static p => {
            if (p.Length < 1)
                return default;
            if (p.Length < 3)
                return p[0];
            return p.ToArray().GroupBy(x => x).OrderByDescending(x => x.Count()).First().Key;
        };
        this.Functions["lerp"] = p => (p[2] - p[1]) * p[0] + p[1];
        this.Functions["inverseLerp"] = static p => (p[0] - p[1]) / (p[2] - p[1]);
    }

    private static T Min(Span<T> values) {
        if (values.Length < 1)
            return default;
        T min = values[0];
        for (int i = 1; i < values.Length; i++)
            min = T.Min(min, values[i]);
        return min;
    }

    private static T Max(Span<T> values) {
        if (values.Length < 1)
            return default;
        T max = values[0];
        for (int i = 1; i < values.Length; i++)
            max = T.Max(max, values[i]);
        return max;
    }

    private static T Sum(Span<T> values) {
        if (values.Length < 1)
            return default;
        T sum = values[0];
        for (int i = 1; i < values.Length; i++)
            sum += values[i];
        return sum;
    }

    public T GetVariable(string identifier) {
        if (this.Variables.TryGetValue(identifier, out T value))
            return value;
        throw new EvaluationException($"Unknown variable: {identifier}");
    }

    public T CallFunction(string identifier, Span<T> parameters) {
        if (this.Functions.TryGetValue(identifier, out MathFunction<T>? function))
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

        this.Variables["pi"] = Math.PI;
        this.Variables["e"] = Math.E;
        this.Variables["tau"] = Math.PI * 2.0;
        this.Variables["phi"] = (1.0 + Math.Sqrt(5.0)) / 2.0;
        this.Variables["inf"] = double.PositiveInfinity;
        this.Variables["nan"] = double.NaN;
        this.Variables["degToRad"] = Math.PI / 180.0;
        this.Variables["radToDeg"] = 180.0 / Math.PI;
        this.Functions["sin"] = static p => Math.Sin(p[0]);
        this.Functions["cos"] = static p => Math.Cos(p[0]);
        this.Functions["tan"] = static p => Math.Tan(p[0]);
        this.Functions["asin"] = static p => Math.Asin(p[0]);
        this.Functions["acos"] = static p => Math.Acos(p[0]);
        this.Functions["atan"] = static p => Math.Atan(p[0]);
        this.Functions["atan2"] = static p => Math.Atan2(p[0], p[1]);
        this.Functions["sinh"] = static p => Math.Sinh(p[0]);
        this.Functions["cosh"] = static p => Math.Cosh(p[0]);
        this.Functions["tanh"] = static p => Math.Tanh(p[0]);
        this.Functions["sqrt"] = static p => Math.Sqrt(p[0]);
        this.Functions["cbrt"] = static p => Math.Cbrt(p[0]);
        this.Functions["root"] = static p => Math.Pow(p[0], 1.0 / p[1]);
        this.Functions["exp"] = static p => Math.Exp(p[0]);
        this.Functions["log"] = static p => Math.Log(p[0]);
        this.Functions["log10"] = static p => Math.Log10(p[0]);
        this.Functions["log2"] = static p => Math.Log2(p[0]);
        this.Functions["logn"] = static p => Math.Log(p[0], p[1]);
        this.Functions["pow"] = static p => Math.Pow(p[0], p[1]);
        this.Functions["floor"] = static p => Math.Floor(p[0]);
        this.Functions["ceil"] = static p => Math.Ceiling(p[0]);
        this.Functions["round"] = static p => Math.Round(p[0]);
    }
}

public sealed class EvaluationContextFloat : EvaluationContext<float> {
    internal EvaluationContextFloat() {
    }

    protected internal override void RegisterBuiltIns() {
        base.RegisterBuiltIns();

        this.Variables["pi"] = (float) (Math.PI);
        this.Variables["e"] = (float) (Math.E);
        this.Variables["tau"] = (float) (Math.PI * 2.0);
        float value = (float) ((1.0 + Math.Sqrt(5.0)) / 2.0);
        this.Variables["phi"] = value;
        this.Variables["inf"] = (float) (double.PositiveInfinity);
        this.Variables["nan"] = (float) (double.NaN);
        this.Variables["degToRad"] = (float) (Math.PI / 180.0);
        this.Variables["radToDeg"] = (float) (180.0 / Math.PI);
        this.Functions["sin"] = static p => (float) Math.Sin(p[0]);
        this.Functions["cos"] = static p => (float) Math.Cos(p[0]);
        this.Functions["tan"] = static p => (float) Math.Tan(p[0]);
        this.Functions["asin"] = static p => (float) Math.Asin(p[0]);
        this.Functions["acos"] = static p => (float) Math.Acos(p[0]);
        this.Functions["atan"] = static p => (float) Math.Atan(p[0]);
        this.Functions["atan2"] = static p => (float) Math.Atan2(p[0], p[1]);
        this.Functions["sinh"] = static p => (float) Math.Sinh(p[0]);
        this.Functions["cosh"] = static p => (float) Math.Cosh(p[0]);
        this.Functions["tanh"] = static p => (float) Math.Tanh(p[0]);
        this.Functions["sqrt"] = static p => (float) Math.Sqrt(p[0]);
        this.Functions["cbrt"] = static p => (float) Math.Cbrt(p[0]);
        this.Functions["root"] = static p => (float) Math.Pow(p[0], 1.0 / p[1]);
        this.Functions["exp"] = static p => (float) Math.Exp(p[0]);
        this.Functions["log"] = static p => (float) Math.Log(p[0]);
        this.Functions["log10"] = static p => (float) Math.Log10(p[0]);
        this.Functions["log2"] = static p => (float) Math.Log2(p[0]);
        this.Functions["logn"] = static p => (float) Math.Log(p[0], p[1]);
        this.Functions["pow"] = static p => (float) Math.Pow(p[0], p[1]);
        this.Functions["floor"] = static p => (float) Math.Floor(p[0]);
        this.Functions["ceil"] = static p => (float) Math.Ceiling(p[0]);
        this.Functions["round"] = static p => (float) Math.Round(p[0]);
    }
}

public sealed class EvaluationContextBinaryNumber<T> : EvaluationContext<T> where T : unmanaged, INumber<T>, IBinaryNumber<T> {
    internal EvaluationContextBinaryNumber() {
    }

    protected internal override void RegisterBuiltIns() {
        base.RegisterBuiltIns();
        this.Functions["log2"] = static p => T.Log2(p[0]);
    }
}