using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ILMath.Compiler;

[SuppressMessage("ReSharper", "StaticMemberInGenericType", Justification = $"We use the {nameof(Util<T>)}<T> class for storing type-specific information")]
public static class Util<T> {
    public static bool IsFP { get; }
    public static bool IsUN { get; }
    public static bool Is64 { get; }
    public static MethodInfo? LeftShiftMethod { get; }
    public static MethodInfo? RightShiftMethod { get; }
    public static MethodInfo? NegateMethod { get; }

    static Util() {
        if (typeof(T) == typeof(int)) {
            return;
        }

        if (typeof(T) == typeof(uint)) {
            IsUN = true;
            NegateMethod = Util.NegateU32;
            return;
        }

        if (typeof(T) == typeof(long)) {
            Is64 = true;
            LeftShiftMethod = Util.LShiftL64;
            RightShiftMethod = Util.RShiftL64;
            return;
        }

        if (typeof(T) == typeof(ulong)) {
            Is64 = true;
            LeftShiftMethod = Util.LShiftUL64;
            RightShiftMethod = Util.RShiftUL64;
            NegateMethod = Util.NegateU64;
            return;
        }

        if (typeof(T) == typeof(float) || (Is64 = typeof(T) == typeof(double))) {
            IsFP = true;
            return;
        }

        throw new InvalidOperationException("Unsupported type: " + typeof(T));
    }
}

public static class Util {
    public static readonly MethodInfo LShiftL64, LShiftUL64;
    public static readonly MethodInfo RShiftL64, RShiftUL64;
    public static readonly MethodInfo NegateU32, NegateU64;

    static Util() {
        LShiftL64 = typeof(Util).GetMethod(nameof(LShift), BindingFlags.Static | BindingFlags.NonPublic, null, new[] { typeof(long), typeof(long) }, null) ?? throw new System.Exception("Missing " + nameof(LShiftL64));
        LShiftUL64 = typeof(Util).GetMethod(nameof(LShift), BindingFlags.Static | BindingFlags.NonPublic, null, new[] { typeof(ulong), typeof(ulong) }, null) ?? throw new System.Exception("Missing " + nameof(LShiftUL64));
        RShiftL64 = typeof(Util).GetMethod(nameof(RShift), BindingFlags.Static | BindingFlags.NonPublic, null, new[] { typeof(long), typeof(long) }, null) ?? throw new System.Exception("Missing " + nameof(RShiftL64));
        RShiftUL64 = typeof(Util).GetMethod(nameof(RShift), BindingFlags.Static | BindingFlags.NonPublic, null, new[] { typeof(ulong), typeof(ulong) }, null) ?? throw new System.Exception("Missing " + nameof(RShiftUL64));
        NegateU32 = typeof(Util).GetMethod(nameof(Negate), BindingFlags.Static | BindingFlags.NonPublic, null, new[] { typeof(uint) }, null) ?? throw new System.Exception("Missing " + nameof(NegateU32));
        NegateU64 = typeof(Util).GetMethod(nameof(Negate), BindingFlags.Static | BindingFlags.NonPublic, null, new[] { typeof(ulong) }, null) ?? throw new System.Exception("Missing " + nameof(NegateU64));
    }

    private static long LShift(long a, long b) => a << (int) Math.Clamp(b, int.MinValue, int.MaxValue);
    private static ulong LShift(ulong a, ulong b) => a << (int) Math.Clamp(b, 0, int.MaxValue);
    private static long RShift(long a, long b) => a >> (int) Math.Clamp(b, int.MinValue, int.MaxValue);
    private static ulong RShift(ulong a, ulong b) => a >> (int) Math.Clamp(b, 0, int.MaxValue);
    private static uint Negate(uint x) => unchecked(0U - x);
    private static ulong Negate(ulong x) => unchecked(0UL - x);
}