namespace ILMath.Test;

[TestClass]
public class EvaluationTestULong {
    [TestMethod]
    public void TestMethod1() {
        RunTestI64("2 + 1 * 5", 7);
    }

    [TestMethod]
    public void TestArithmetic() {
        RunTestI64("1 + 2", 1 + 2);
        RunTestI64("10 - 3", 10 - 3);
        RunTestI64("4 * 5", 4 * 5);
        RunTestI64("20 / 4", 20 / 4);
        RunTestI64("7 % 3", 7 % 3);
    }

    [TestMethod]
    public void TestPrecedence() {
        RunTestI64("2 + 3 * 4", 2 + 3 * 4);
        RunTestI64("(2 + 3) * 4", (2 + 3) * 4);
        RunTestI64("2 + 3 * 4 - 5", 2 + 3 * 4 - 5);
        RunTestI64("10 - 2 * 3 + 4", 10 - 2 * 3 + 4);
    }

    [TestMethod]
    public void TestUnary() {
        RunTestI64("-5", unchecked((ulong) -5));
        RunTestI64("+5", +5);
        RunTestI64("~0", unchecked((ulong) ~0));
        RunTestI64("~-1", ~-1);
        RunTestI64("~+1", unchecked((ulong) ~+1));
        RunTestI64("-(3 + 4)", unchecked((ulong) -(3 + 4)));
        RunTestI64("-(-5)", -(-5));
    }

    [TestMethod]
    public void TestBinaryOperators() {
        RunTestI64("2 ^ 3", 2 ^ 3); // bitwise XOR for ulong
        RunTestI64("2 ^ 3 ^ 1", 2 ^ 3 ^ 1); // right-associative
        RunTestI64("~(2 ^ 3)", unchecked((ulong) ~(2 ^ 3)));
    }

    [TestMethod]
    public void TestModDiv() {
        RunTestI64("10 % 3 * 4", 10 % 3 * 4);
        RunTestI64("10 / 2 % 3", 10 / 2 % 3);
    }

    [TestMethod]
    public void TestShifting() {
        RunTestI64("256 >> 4", 256 >> 4);
        RunTestI64("256 >> 4 << 2", 256 >> 4 << 2);
        RunTestI64("(256 >> 4) << 2", (256 >> 4) << 2);
        RunTestI64("8 << 1 + 1", 8 << 1 + 1);
        RunTestI64("8 << (1 + 1)", 8 << (1 + 1));
    }

    [TestMethod]
    public void TestBitwise() {
        RunTestI64("5 & 3", 5 & 3);
        RunTestI64("5 | 3", 5 | 3);
        RunTestI64("5 | 3 & 1", 5 | 3 & 1);
        RunTestI64("(5 | 3) & 1", (5 | 3) & 1);
        RunTestI64("5 & 3 | 1", 5 & 3 | 1);
        RunTestI64("1 + 2 << 3", (1 + 2) << 3);
        RunTestI64("4 * 2 | 1", 4 * 2 | 1);
        RunTestI64("16 >> 2 + 1", 16 >> 2 + 1);
        RunTestI64("(16 >> 2) + 1", (16 >> 2) + 1);
    }

    [TestMethod]
    public void TestParentheses() {
        RunTestI64("(((2 + 3)))", (((2 + 3))));
        RunTestI64("((2 + 3) * (4 + 5))", ((2 + 3) * (4 + 5)));
    }

    [TestMethod]
    public void TestFunctions() {
        RunTestI64("f(3)", 3 * 2);
        RunTestI64("f(2 + 3)", (2 + 3) * 2);
        RunTestI64("f(f(2))", (2 * 2) * 2);
        RunTestI64("f(2 + f(3))", (2 + (3 * 2)) * 2);
    }

    [TestMethod]
    public void TestGeneral() {
        RunTestI64("1 + 2 * 3 ^ 4", 1 + 2 * 3 ^ 4);
        RunTestI64("(1 + 2) * (3 ^ 4)", (1 + 2) * (3 ^ 4));
        RunTestI64("1 + 2 << 3 ^ 1", 1 + 2 << 3 ^ 1);
        RunTestI64("~(1 + 2) << 3", unchecked((ulong) (~(1 + 2) << 3)));
        RunTestI64("2 ^ 3 ^ 4", 2 ^ 3 ^ 4);
        RunTestI64("100 / 10 / 2", 100 / 10 / 2);
        RunTestI64("100 - 10 - 5", 100 - 10 - 5);
        RunTestI64("1 << 2 << 3", 1 << 2 << 3);
    }

    [TestMethod]
    public void TestTabsAndWhiteSpaces() {
        RunTestI64("  1 +   2  * 3  ", 1 + 2 * 3);
        RunTestI64("\t(4+5\t )*2", (4 + 5) * 2);
    }

    private static readonly CompilationMethod[] Methods = {
        CompilationMethod.ExpressionTree, CompilationMethod.Functional, CompilationMethod.IntermediateLanguage
    };

    private static void RunTestI64(string expression, ulong expected) {
        foreach (CompilationMethod method in Methods) {
            Evaluator<ulong> compiled = MathEvaluation.CompileExpression<ulong>(string.Empty, expression, method);
            EvaluationContext<ulong> context = EvaluationContexts.CreateForInteger<ulong>();
            context.SetFunction("f", 1, static p => p[0] * 2);
            Assert.AreEqual(expected, compiled(context), $"Test failed for compilation method: {method}");
        }
    }
}