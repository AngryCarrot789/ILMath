using System.Numerics;
using System.Runtime.CompilerServices;
using ILMath.Exception;

namespace ILMath;

/// <summary>
/// Default implementation of <see cref="IEvaluationContext{T}"/>.
/// </summary>
public abstract class EvaluationContext<T> : IEvaluationContext<T> where T : unmanaged, INumber<T> {
    private static readonly Dictionary<string, FunctionDeclaration<T>> globalFunctions = new Dictionary<string, FunctionDeclaration<T>>();
    private static readonly Dictionary<string, T> globalVariables = new Dictionary<string, T>();

    private Dictionary<string, FunctionDeclaration<T>>? myFunctions;
    private Dictionary<string, T>? myVariables;

    protected EvaluationContext() {
    }

    static EvaluationContext() {
        SetGlobalFunction("mod", 2, static p => p[0] % p[1]);
        SetGlobalFunction("abs", 1, static p => T.Abs(p[0]));
        SetGlobalFunction("min", Min);
        SetGlobalFunction("max", Max);
        SetGlobalFunction("sum", Sum);
        SetGlobalFunction("mean", static p => Sum(p) / T.CreateChecked(p.Length));
        SetGlobalFunction("average", static p => Sum(p) / T.CreateChecked(p.Length));
        SetGlobalFunction("range", static p => {
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
        });

        SetGlobalFunction("clamp", 3, static p => T.Clamp(p[0], p[1], p[2]));
        SetGlobalFunction("mode", static p => {
            if (p.Length < 1)
                return default;
            if (p.Length < 3)
                return p[0];
            return p.ToArray().GroupBy(x => x).OrderByDescending(x => x.Count()).First().Key;
        });

        SetGlobalFunction("lerp", 3, p => (p[2] - p[1]) * p[0] + p[1]);
        SetGlobalFunction("inverseLerp", 3, static p => (p[0] - p[1]) / (p[2] - p[1]));
    }

    public void SetFunction(string name, EvaluationFunction<T> function) {
        (this.myFunctions ??= new Dictionary<string, FunctionDeclaration<T>>())[name] = new FunctionDeclaration<T>(function);
    }

    public void SetFunction(string name, int requiredArgs, EvaluationFunction<T> function) {
        (this.myFunctions ??= new Dictionary<string, FunctionDeclaration<T>>())[name] = new FunctionDeclaration<T>(function, requiredArgs, requiredArgs);
    }

    public void SetFunction(string name, int minParams, int maxParams, EvaluationFunction<T> function) {
        (this.myFunctions ??= new Dictionary<string, FunctionDeclaration<T>>())[name] = new FunctionDeclaration<T>(function, minParams, maxParams);
    }

    public void SetVariable(string name, T value) {
        (this.myVariables ??= new Dictionary<string, T>())[name] = value;
    }

    public static void SetGlobalFunction(string name, EvaluationFunction<T> function) {
        globalFunctions[name] = new FunctionDeclaration<T>(function);
    }

    public static void SetGlobalFunction(string name, int requiredArgs, EvaluationFunction<T> function) {
        globalFunctions[name] = new FunctionDeclaration<T>(function, requiredArgs, requiredArgs);
    }

    public static void SetGlobalFunction(string name, int minParams, int maxParams, EvaluationFunction<T> function) {
        globalFunctions[name] = new FunctionDeclaration<T>(function, minParams, maxParams);
    }

    public static void SetGlobalVariable(string name, T value) {
        globalVariables[name] = value;
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
        if (this.TryGetVariable(identifier, out T value))
            return value;
        throw new EvaluationException($"Unknown variable: {identifier}");
    }

    public T CallFunction(string identifier, Span<T> parameters) {
        if (!this.TryGetFunction(identifier, out FunctionDeclaration<T> function))
            throw new EvaluationException($"Unknown function: {identifier}");

        if (function.MinParams != -1 && parameters.Length < function.MinParams)
            throw new EvaluationException($"Not enough args. Need at least {function.MinParams}, got {parameters.Length}");

        if (function.MaxParams != -1 && parameters.Length > function.MaxParams)
            throw new EvaluationException($"Too many args. Maximum is {function.MaxParams}, got {parameters.Length}");

        return function.Function(parameters);
    }

    public bool TryGetVariable(string identifier, out T variable) {
        if (this.myVariables != null && this.myVariables.TryGetValue(identifier, out variable))
            return true;
        if (globalVariables.TryGetValue(identifier, out variable))
            return true;
        return false;
    }

    public bool TryGetFunction(string identifier, out FunctionDeclaration<T> declaration) {
        if (this.myFunctions != null && this.myFunctions.TryGetValue(identifier, out declaration))
            return true;
        if (globalFunctions.TryGetValue(identifier, out declaration))
            return true;
        return false;
    }
}

public static class EvaluationContexts {
    public static EvaluationContext<double> CreateForDouble() {
        return new EvaluationContextDouble();
    }

    public static EvaluationContext<float> CreateForFloat() {
        return new EvaluationContextFloat();
    }

    public static EvaluationContext<T> CreateForInteger<T>() where T : unmanaged, INumber<T>, IBinaryInteger<T> {
        return new EvaluationContextBinaryNumber<T>();
    }
}

public sealed class EvaluationContextDouble : EvaluationContext<double> {
    internal EvaluationContextDouble() {
    }

    static EvaluationContextDouble() {
        SetGlobalVariable("pi", Math.PI);
        SetGlobalVariable("e", Math.E);
        SetGlobalVariable("tau", Math.PI * 2.0);
        SetGlobalVariable("phi", (1.0 + Math.Sqrt(5.0)) / 2.0);
        SetGlobalVariable("inf", double.PositiveInfinity);
        SetGlobalVariable("nan", double.NaN);
        SetGlobalVariable("degToRad", Math.PI / 180.0);
        SetGlobalVariable("radToDeg", 180.0 / Math.PI);
        SetGlobalFunction("sin", 1, static p => Math.Sin(p[0]));
        SetGlobalFunction("cos", 1, static p => Math.Cos(p[0]));
        SetGlobalFunction("tan", 1, static p => Math.Tan(p[0]));
        SetGlobalFunction("asin", 1, static p => Math.Asin(p[0]));
        SetGlobalFunction("acos", 1, static p => Math.Acos(p[0]));
        SetGlobalFunction("atan", 1, static p => Math.Atan(p[0]));
        SetGlobalFunction("atan2", 2, static p => Math.Atan2(p[0], p[1]));
        SetGlobalFunction("sinh", 1, static p => Math.Sinh(p[0]));
        SetGlobalFunction("cosh", 1, static p => Math.Cosh(p[0]));
        SetGlobalFunction("tanh", 1, static p => Math.Tanh(p[0]));
        SetGlobalFunction("sqrt", 1, static p => Math.Sqrt(p[0]));
        SetGlobalFunction("cbrt", 1, static p => Math.Cbrt(p[0]));
        SetGlobalFunction("root", 2, static p => Math.Pow(p[0], 1.0 / p[1]));
        SetGlobalFunction("exp", 1, static p => Math.Exp(p[0]));
        SetGlobalFunction("log", 1, static p => Math.Log(p[0]));
        SetGlobalFunction("log10", 1, static p => Math.Log10(p[0]));
        SetGlobalFunction("log2", 1, static p => Math.Log2(p[0]));
        SetGlobalFunction("logn", 2, static p => Math.Log(p[0], p[1]));
        SetGlobalFunction("pow", 2, static p => Math.Pow(p[0], p[1]));
        SetGlobalFunction("floor", 1, static p => Math.Floor(p[0]));
        SetGlobalFunction("ceil", 1, static p => Math.Ceiling(p[0]));
        SetGlobalFunction("round", 1, 2, static p => p.Length == 1 ? Math.Round(p[0]) : Math.Round(p[0], Math.Clamp((int) p[1], 0, 15)));
        SetGlobalFunction("rand", 1, 2, static p => p.Length == 1
            ? Random.Shared.NextDouble() * p[1]
            : Random.Shared.NextDouble() * (p[1] - p[0]) + p[0]);
    }
}

public sealed class EvaluationContextFloat : EvaluationContext<float> {
    internal EvaluationContextFloat() {
    }

    static EvaluationContextFloat() {
        SetGlobalVariable("pi", (float) (Math.PI));
        SetGlobalVariable("e", (float) (Math.E));
        SetGlobalVariable("tau", (float) (Math.PI * 2.0));
        SetGlobalVariable("phi", (float) ((1.0 + Math.Sqrt(5.0)) / 2.0));
        SetGlobalVariable("inf", (float) (double.PositiveInfinity));
        SetGlobalVariable("nan", (float) (double.NaN));
        SetGlobalVariable("degToRad", (float) (Math.PI / 180.0));
        SetGlobalVariable("radToDeg", (float) (180.0 / Math.PI));
        SetGlobalFunction("sin", 1, static p => (float) Math.Sin(p[0]));
        SetGlobalFunction("cos", 1, static p => (float) Math.Cos(p[0]));
        SetGlobalFunction("tan", 1, static p => (float) Math.Tan(p[0]));
        SetGlobalFunction("asin", 1, static p => (float) Math.Asin(p[0]));
        SetGlobalFunction("acos", 1, static p => (float) Math.Acos(p[0]));
        SetGlobalFunction("atan", 1, static p => (float) Math.Atan(p[0]));
        SetGlobalFunction("atan2", 2, static p => (float) Math.Atan2(p[0], p[1]));
        SetGlobalFunction("sinh", 1, static p => (float) Math.Sinh(p[0]));
        SetGlobalFunction("cosh", 1, static p => (float) Math.Cosh(p[0]));
        SetGlobalFunction("tanh", 1, static p => (float) Math.Tanh(p[0]));
        SetGlobalFunction("sqrt", 1, static p => (float) Math.Sqrt(p[0]));
        SetGlobalFunction("cbrt", 1, static p => (float) Math.Cbrt(p[0]));
        SetGlobalFunction("root", 2, static p => (float) Math.Pow(p[0], 1.0 / p[1]));
        SetGlobalFunction("exp", 1, static p => (float) Math.Exp(p[0]));
        SetGlobalFunction("log", 1, static p => (float) Math.Log(p[0]));
        SetGlobalFunction("log10", 1, static p => (float) Math.Log10(p[0]));
        SetGlobalFunction("log2", 1, static p => (float) Math.Log2(p[0]));
        SetGlobalFunction("logn", 2, static p => (float) Math.Log(p[0], p[1]));
        SetGlobalFunction("pow", 2, static p => (float) Math.Pow(p[0], p[1]));
        SetGlobalFunction("floor", 1, static p => (float) Math.Floor(p[0]));
        SetGlobalFunction("ceil", 1, static p => (float) Math.Ceiling(p[0]));
        SetGlobalFunction("round", 1, 2, static p => (float) (p.Length == 1 ? Math.Round(p[0]) : Math.Round(p[0], Math.Clamp((int) p[1], 0, 15))));
        SetGlobalFunction("rand", 1, 2, static p => (float) (p.Length == 1
            ? Random.Shared.NextDouble() * p[1]
            : Random.Shared.NextDouble() * (p[1] - p[0]) + p[0]));
    }
}

public sealed class EvaluationContextBinaryNumber<T> : EvaluationContext<T> where T : unmanaged, INumber<T>, IBinaryInteger<T> {
    internal EvaluationContextBinaryNumber() {
    }

    static EvaluationContextBinaryNumber() {
        SetGlobalFunction("log2", 1, static p => T.Log2(p[0]));
        SetGlobalFunction("rand", 1, 2, static p => p.Length == 1 ? RandomInclusive(T.Zero, p[0]) : RandomInclusive(p[0], p[1]));
    }

    private static T RandomInclusive(T min, T max) {
        if (min > max) {
            (min, max) = (max, min);
        }
        
        if (typeof(T) == typeof(int)) {
            long result = Random.Shared.NextInt64(Unsafe.As<T, int>(ref min), (long) Unsafe.As<T, int>(ref max) + 1);
            int value = (int) result;
            return Unsafe.As<int, T>(ref value);
        }
        else if (typeof(T) == typeof(uint)) {
            ulong result = (ulong) Random.Shared.NextInt64(Unsafe.As<T, uint>(ref min), (long) Unsafe.As<T, uint>(ref max) + 1);
            uint value = (uint) result;
            return Unsafe.As<uint, T>(ref value);
        }
        else if (typeof(T) == typeof(long)) {
            long max64 = Unsafe.As<T, long>(ref max);
            long value = Random.Shared.NextInt64(Unsafe.As<T, long>(ref min), max64 == long.MaxValue ? max64 : max64 + 1);
            return Unsafe.As<long, T>(ref value);
        }
        else if (typeof(T) == typeof(ulong)) {
            ulong max64 = Unsafe.As<T, ulong>(ref max);
            ulong value = (ulong) Random.Shared.NextInt64(
                (long) Math.Min(Unsafe.As<T, ulong>(ref min), long.MaxValue),
                (long) Math.Min(max64 == ulong.MaxValue ? max64 : (max64 + 1), long.MaxValue)
            );
            
            return Unsafe.As<ulong, T>(ref value);
        }

        throw new InvalidOperationException($"Unsupported type: {typeof(T)}");
    }
}