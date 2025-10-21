namespace ILMath.Test;

[TestClass]
public class EvaluationTestFloatingPoint {
    [TestMethod]
    public void TestMethod1() {
        RunTestD64("2 + 1 * 5", 7.0);
    }

    [TestMethod]
    public void TestMethod2() {
        RunTestD64("sin(pi)", 0.0);
    }

    [TestMethod]
    public void TestMethod3() {
        RunTestD64("sin(pi / 2)", 1.0);
    }

    [TestMethod]
    public void TestMethod4() {
        RunTestD64("cos(pi / 4) * 8.0", Math.Sqrt(2.0) * 4.0);
    }

    [TestMethod]
    public void TestMethod5() {
        RunTestD64("4.0 * sin(pi / 4) * 8.0 + 1.0", Math.Sqrt(2.0) * 16.0 + 1.0);
    }

    [TestMethod]
    public void TestMethod6() {
        RunTestD64("pow(4, 7)", Math.Pow(4.0, 7.0));
    }

    [TestMethod]
    public void TestMethod7() {
        RunTestD64("pow(4, 7 + 2)", Math.Pow(4.0, 7.0 + 2.0));
    }

    [TestMethod]
    public void TestMethod8() {
        RunTestD64("pow(4, pow(7 + 2, 1.5))", Math.Pow(4.0, Math.Pow(7.0 + 2.0, 1.5)));
    }

    [TestMethod]
    public void TestArithmetic() {
        RunTestD64("1 + 2", 1 + 2);
        RunTestD64("10 - 3", 10 - 3);
        RunTestD64("4 * 5", 4 * 5);
        RunTestD64("20 / 4", 20.0 / 4.0);
        RunTestD64("7 % 3", 7 % 3);
    }

    [TestMethod]
    public void TestPrecedence() {
        RunTestD64("2 + 3 * 4", 2 + 3 * 4);
        RunTestD64("(2 + 3) * 4", (2 + 3) * 4);
        RunTestD64("2 + 3 * 4 - 5", 2 + 3 * 4 - 5);
        RunTestD64("10 - 2 * 3 + 4", 10 - 2 * 3 + 4);
    }

    [TestMethod]
    public void TestUnary() {
        RunTestD64("-5", -5);
        RunTestD64("+5", +5);
        RunTestD64("-(3 + 4)", -(3 + 4));
        RunTestD64("-(-5)", -(-5));
    }

    [TestMethod]
    public void TestModDiv() {
        RunTestD64("10 % 3 * 4", 10.0 % 3 * 4.0);
        RunTestD64("10 / 2 % 3", 10.0 / 2.0 % 3.0);
    }

    [TestMethod]
    public void TestParentheses() {
        RunTestD64("(((2 + 3)))", (((2 + 3))));
        RunTestD64("((2 + 3) * (4 + 5))", ((2 + 3) * (4 + 5)));
    }

    [TestMethod]
    public void TestFunctions() {
        RunTestD64("f(3)", 3 * 2);
        RunTestD64("f(2 + 3)", (2 + 3) * 2);
        RunTestD64("f(f(2))", (2 * 2) * 2);
        RunTestD64("f(2 + f(3))", (2 + (3 * 2)) * 2);
    }

    [TestMethod]
    public void TestTabsAndWhiteSpaces() {
        RunTestD64("  1 +   2  * 3  ", 1 + 2 * 3);
        RunTestD64("\t(4+5\t )*2", (4 + 5) * 2);
    }

    private static readonly CompilationMethod[] Methods = {
        CompilationMethod.ExpressionTree, CompilationMethod.Functional, CompilationMethod.IntermediateLanguage
    };

    private static void RunTestD64(string expression, double expected) {
        foreach (CompilationMethod method in Methods) {
            Evaluator<double> compiled = MathEvaluation.CompileExpression<double>(string.Empty, expression, method);
            EvaluationContext<double> context = EvaluationContexts.CreateForDouble();
            context.SetFunction("f", 1, static p => p[0] * 2);
            Assert.AreEqual(expected, compiled(context), 0.00001, $"Test failed for compilation method: {method}");
        }
    }
}