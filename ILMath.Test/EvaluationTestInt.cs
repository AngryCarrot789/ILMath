namespace ILMath.Test;

[TestClass]
public class EvaluationTestInt {
    [TestMethod]
    public void TestMethod1() {
        RunTestI32("2 + 1 * 5", 7);
    }

    [TestMethod]
    public void TestArithmetic() {
        RunTestI32("1 + 2", 1 + 2);
        RunTestI32("10 - 3", 10 - 3);
        RunTestI32("4 * 5", 4 * 5);
        RunTestI32("20 / 4", 20 / 4);
        RunTestI32("7 % 3", 7 % 3);
    }

    [TestMethod]
    public void TestPrecedence() {
        RunTestI32("2 + 3 * 4", 2 + 3 * 4);
        RunTestI32("(2 + 3) * 4", (2 + 3) * 4);
        RunTestI32("2 + 3 * 4 - 5", 2 + 3 * 4 - 5);
        RunTestI32("10 - 2 * 3 + 4", 10 - 2 * 3 + 4);
    }

    [TestMethod]
    public void TestUnary() {
        RunTestI32("-5", -5);
        RunTestI32("+5", +5);
        RunTestI32("~0", ~0);
        RunTestI32("~-1", ~-1);
        RunTestI32("~+1", ~+1);
        RunTestI32("-(3 + 4)", -(3 + 4));
        RunTestI32("-(-5)", -(-5));
    }

    [TestMethod]
    public void TestBinaryOperators() {
        RunTestI32("2 ^ 3", 2 ^ 3); // bitwise XOR for int
        RunTestI32("2 ^ 3 ^ 1", 2 ^ 3 ^ 1); // right-associative
        RunTestI32("~(2 ^ 3)", ~(2 ^ 3));
    }

    [TestMethod]
    public void TestModDiv() {
        RunTestI32("10 % 3 * 4", 10 % 3 * 4);
        RunTestI32("10 / 2 % 3", 10 / 2 % 3);
    }

    [TestMethod]
    public void TestShifting() {
        RunTestI32("256 >> 4", 256 >> 4);
        RunTestI32("256 >> 4 << 2", 256 >> 4 << 2);
        RunTestI32("(256 >> 4) << 2", (256 >> 4) << 2);
        RunTestI32("8 << 1 + 1", 8 << 1 + 1);
        RunTestI32("8 << (1 + 1)", 8 << (1 + 1));
    }

    [TestMethod]
    public void TestBitwise() {
        RunTestI32("5 & 3", 5 & 3);
        RunTestI32("5 | 3", 5 | 3);
        RunTestI32("5 | 3 & 1", 5 | 3 & 1);
        RunTestI32("(5 | 3) & 1", (5 | 3) & 1);
        RunTestI32("5 & 3 | 1", 5 & 3 | 1);
        RunTestI32("1 + 2 << 3", (1 + 2) << 3);
        RunTestI32("4 * 2 | 1", 4 * 2 | 1);
        RunTestI32("16 >> 2 + 1", 16 >> 2 + 1);
        RunTestI32("(16 >> 2) + 1", (16 >> 2) + 1);
        RunTestI32("5 * ~10 ^ 5 / +15 & 0x7 + 20", 5 * ~10 ^ 5 / +15 & 0x7 + 20);
    }

    [TestMethod]
    public void TestParentheses() {
        RunTestI32("(((2 + 3)))", (((2 + 3))));
        RunTestI32("((2 + 3) * (4 + 5))", ((2 + 3) * (4 + 5)));
    }

    [TestMethod]
    public void TestFunctions() {
        RunTestI32("f(3)", 3 * 2);
        RunTestI32("f(2 + 3)", (2 + 3) * 2);
        RunTestI32("f(f(2))", (2 * 2) * 2);
        RunTestI32("f(2 + f(3))", (2 + (3 * 2)) * 2);
    }

    [TestMethod]
    public void TestGeneral() {
        RunTestI32("1 + 2 * 3 ^ 4", 1 + 2 * 3 ^ 4);
        RunTestI32("(1 + 2) * (3 ^ 4)", (1 + 2) * (3 ^ 4));
        RunTestI32("1 + 2 << 3 ^ 1", 1 + 2 << 3 ^ 1);
        RunTestI32("~(1 + 2) << 3", ~(1 + 2) << 3);
        RunTestI32("2 ^ 3 ^ 4", 2 ^ 3 ^ 4);
        RunTestI32("100 / 10 / 2", 100 / 10 / 2);
        RunTestI32("100 - 10 - 5", 100 - 10 - 5);
        RunTestI32("1 << 2 << 3", 1 << 2 << 3);
    }

    [TestMethod]
    public void TestHexParsing() {
        RunTestI32("0x0101", 0x0101);
        RunTestI32("0X7FFFFFFF", 0X7FFFFFFF);
    }

    [TestMethod]
    public void TestTabsAndWhiteSpaces() {
        RunTestI32("  1 +   2  * 3  ", 1 + 2 * 3);
        RunTestI32("\t(4+5\t )*2", (4 + 5) * 2);
    }

    // Order from easiest to hardest to debug :P
    private static readonly CompilationMethod[] Methods = {
        CompilationMethod.ExpressionTree, CompilationMethod.Functional, CompilationMethod.IntermediateLanguage
    };

    private static void RunTestI32(string expression, int expected) {
        foreach (CompilationMethod method in Methods) {
            Evaluator<int> compiled = MathEvaluation.CompileExpression<int>(string.Empty, expression, new ParsingContext(), method);
            EvaluationContext<int> context = EvaluationContexts.CreateForInteger<int>();
            context.SetFunction("f", 1, static p => p[0] * 2);
            Assert.AreEqual(expected, compiled(context), $"Test failed for compilation method: {method}");
        }
    }
}