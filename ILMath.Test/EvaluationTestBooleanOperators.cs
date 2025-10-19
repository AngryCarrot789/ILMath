namespace ILMath.Test;

[TestClass]
public class EvaluationTestBooleanOperators {
    [TestMethod]
    public void TestArithmetic() {
        /*
         * Expected results are calculated from C++ output:
         *     std::cout << "[(0)]        "<< (0)        << " should equal " << 0 << "\n";
         *     std::cout << "[(5)]        "<< (5)        << " should equal " << 5 << "\n";
         *     std::cout << "[(!0)]       "<< (!0)       << " should equal " << 1 << "\n";
         *     std::cout << "[(!5)]       "<< (!5)       << " should equal " << 0 << "\n";
         *     std::cout << "[(!-5)]      "<< (!-5)      << " should equal " << 0 << "\n";
         *     std::cout << "[(!5-5)]     "<< (!5-5)     << " should equal " << -5 << "\n";
         *     std::cout << "[(!5*5)]     "<< (!5*5)     << " should equal " << 0 << "\n";
         *     std::cout << "[(!!0)]      "<< (!!0)      << " should equal " << 0 << "\n";
         *     std::cout << "[(!!5)]      "<< (!!5)      << " should equal " << 1 << "\n";
         *     std::cout << "[(!!-5)]     "<< (!!-5)     << " should equal " << 1 << "\n";
         *     std::cout << "[(!!5-5)]    "<< (!!5-5)    << " should equal " << -4 << "\n";
         *     std::cout << "[(!!5*5)]    "<< (!!5*5)    << " should equal " << 5 << "\n";
         *     std::cout << "[(!0)]       "<< (!0)       << " should equal " << 1 << "\n";
         *     std::cout << "[(!1)]       "<< (!1)       << " should equal " << 0 << "\n";
         *     std::cout << "[(!5)]       "<< (!5)       << " should equal " << 0 << "\n";
         *     std::cout << "[(!(2 + 2))] "<< (!(2 + 2)) << " should equal " << 0 << "\n";
         *     std::cout << "[(!0 + 1)]   "<< (!0 + 1)   << " should equal " << 2 << "\n";
         *     std::cout << "[(!1 * 10)]  "<< (!1 * 10)  << " should equal " << 0 << "\n";
         *     std::cout << "[(!~0)]      "<< (!~0)      << " should equal " << 0 << "\n";
         *     std::cout << "[(!!0)]      "<< (!!0)      << " should equal " << 0 << "\n";
         */


        RunTestI32("0", 0);
        RunTestI32("5", 5);
        RunTestI32("!0", 1);
        RunTestI32("!5", 0);
        RunTestI32("!-5", 0);
        RunTestI32("!5-5", -5);
        RunTestI32("!5*5", 0);
        RunTestI32("!!0", 0);
        RunTestI32("!!5", 1);
        RunTestI32("!!-5", 1);
        RunTestI32("!!5-5", -4);
        RunTestI32("!!5*5", 5);
        RunTestI32("!0", 1);
        RunTestI32("!1", 0);
        RunTestI32("!5", 0);
        RunTestI32("!(2 + 2)", 0);
        RunTestI32("!0 + 1", 2);
        RunTestI32("!1 * 10", 0);
        RunTestI32("!~0", 0);
        RunTestI32("!!0", 0);
    }
    
    [TestMethod]
    public void TestEquality() {
        RunTestI32("-1 == 0", (-1 == 0) ? 1 : 0);
        RunTestI32("1 == -1", (1 == -1) ? 1 : 0);
        RunTestI32("0 == 0", (0 == 0) ? 1 : 0);
        RunTestI32("1 == 0", (1 == 0) ? 1 : 0);
        RunTestI32("1 == 1", (1 == 1) ? 1 : 0);
        RunTestI32("5 == 5", (5 == 5) ? 1 : 0);
        RunTestI32("9 == 3", (9 == 3) ? 1 : 0);
        RunTestI32("4 == 20", (4 == 20) ? 1 : 0);
    }
    
    [TestMethod]
    public void TestInequality() {
        RunTestI32("-1 != 0", (-1 != 0) ? 1 : 0);
        RunTestI32("1 != -1", (1 != -1) ? 1 : 0);
        RunTestI32("0 != 0", (0 != 0) ? 1 : 0);
        RunTestI32("1 != 0", (1 != 0) ? 1 : 0);
        RunTestI32("1 != 1", (1 != 1) ? 1 : 0);
        RunTestI32("5 != 5", (5 != 5) ? 1 : 0);
        RunTestI32("9 != 3", (9 != 3) ? 1 : 0);
        RunTestI32("4 != 20", (4 != 20) ? 1 : 0);
        RunTestI32("4 != 4", (4 != 4) ? 1 : 0);
    }

    [TestMethod]
    public void TestRelational() {
        RunTestI32("-1 > 0", (-1 > 0) ? 1 : 0);
        RunTestI32("1 > -1", (1 > -1) ? 1 : 0);
        RunTestI32("0 > 0", (0 > 0) ? 1 : 0);
        RunTestI32("1 > 0", (1 > 0) ? 1 : 0);
        RunTestI32("1 > 1", (1 > 1) ? 1 : 0);
        RunTestI32("5 > 5", (5 > 5) ? 1 : 0);
        RunTestI32("9 > 3", (9 > 3) ? 1 : 0);
        RunTestI32("4 > 20", (4 > 20) ? 1 : 0);
        
        RunTestI32("-1 >= 0", (-1 >= 0) ? 1 : 0);
        RunTestI32("1 >= -1", (1 >= -1) ? 1 : 0);
        RunTestI32("0 >= 0", (0 >= 0) ? 1 : 0);
        RunTestI32("1 >= 0", (1 >= 0) ? 1 : 0);
        RunTestI32("1 >= 1", (1 >= 1) ? 1 : 0);
        RunTestI32("5 >= 5", (5 >= 5) ? 1 : 0);
        RunTestI32("9 >= 3", (9 >= 3) ? 1 : 0);
        RunTestI32("4 >= 20", (4 >= 20) ? 1 : 0);
        
        RunTestI32("-1 < 0", (-1 < 0) ? 1 : 0);
        RunTestI32("1 < -1", (1 < -1) ? 1 : 0);
        RunTestI32("0 < 0", (0 < 0) ? 1 : 0);
        RunTestI32("1 < 0", (1 < 0) ? 1 : 0);
        RunTestI32("1 < 1", (1 < 1) ? 1 : 0);
        RunTestI32("5 < 5", (5 < 5) ? 1 : 0);
        RunTestI32("9 < 3", (9 < 3) ? 1 : 0);
        RunTestI32("4 < 20", (4 < 20) ? 1 : 0);
        
        RunTestI32("-1 <= 0", (-1 <= 0) ? 1 : 0);
        RunTestI32("1 <= -1", (1 <= -1) ? 1 : 0);
        RunTestI32("0 <= 0", (0 <= 0) ? 1 : 0);
        RunTestI32("1 <= 0", (1 <= 0) ? 1 : 0);
        RunTestI32("1 <= 1", (1 <= 1) ? 1 : 0);
        RunTestI32("5 <= 5", (5 <= 5) ? 1 : 0);
        RunTestI32("9 <= 3", (9 <= 3) ? 1 : 0);
        RunTestI32("4 <= 20", (4 <= 20) ? 1 : 0);
    }
    
    [TestMethod]
    public void TestConditionalAndOr() {
        RunTestI32("0 == 0 && 1 == 0", (0 == 0 && 1 == 0) ? 1 : 0);
        RunTestI32("1 == 0 && 1 == 0", (1 == 0 && 1 == 0) ? 1 : 0);
        RunTestI32("1 == 1 && 1 == 0", (1 == 1 && 1 == 0) ? 1 : 0);
        RunTestI32("1 == 1 && 1 == 1", (1 == 1 && 1 == 1) ? 1 : 0);
        
        RunTestI32("4 < 5 && 5 > 10", (4 < 5 && 5 > 10) ? 1 : 0);
        RunTestI32("2 < 5 && 5 > 10", (2 < 5 && 5 > 10) ? 1 : 0);
        RunTestI32("9 < 5 && 5 > 10", (9 < 5 && 5 > 10) ? 1 : 0);
        RunTestI32("4 < 5 && 20 > 10", (4 < 5 && 20 > 10) ? 1 : 0);
    }

    private static readonly CompilationMethod[] Methods = {
        CompilationMethod.ExpressionTree, CompilationMethod.Functional, CompilationMethod.IntermediateLanguage
    };

    private static void RunTestI32(string expression, int expected) {
        foreach (CompilationMethod method in Methods) {
            Evaluator<int> compiled = MathEvaluation.CompileExpression<int>(string.Empty, expression, method);
            EvaluationContext<int> context = EvaluationContexts.CreateForInteger<int>();
            context.Functions["f"] = static p => p[0] * 2;

            Assert.AreEqual(expected, compiled(context), $"Test failed for compilation method: {method}");
        }
    }
}