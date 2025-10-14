using ILMath.SyntaxTree;

namespace ILMath.Test;

[TestClass]
public class EqualityTestInt {
    [TestMethod]
    public void TestMethod1() {
        OperatorNode expect = new OperatorNode(
            OperatorType.Plus,
            new NumberNode<int>(2),
            new OperatorNode(
                OperatorType.Multiplication,
                new NumberNode<int>(1),
                new NumberNode<int>(5)));
        Assert.AreEqual(expect, CreateSyntaxTree("2 + 1 * 5"));
    }

    [TestMethod]
    public void TestMethod2() {
        FunctionNode expect = new FunctionNode("sin", [new VariableNode("pi")]);
        Assert.AreEqual(expect, CreateSyntaxTree("sin(pi)"));
    }

    [TestMethod]
    public void TestMethod3() {
        FunctionNode expect = new FunctionNode(
            "sin", [
                new OperatorNode(
                    OperatorType.Division,
                    new VariableNode("pi"),
                    new NumberNode<int>(2))
            ]);
        Assert.AreNotEqual(expect, CreateSyntaxTree("sin(pi / 4)"));
    }

    [TestMethod]
    public void TestMethod4() {
        OperatorNode expect = new OperatorNode(
            OperatorType.Multiplication,
            new FunctionNode(
                "sin", [
                    new OperatorNode(
                        OperatorType.Division,
                        new VariableNode("pi"),
                        new NumberNode<int>(4))
                ]),
            new NumberNode<int>(8));
        Assert.AreEqual(expect, CreateSyntaxTree("sin(pi / 4) * 8"));
    }

    [TestMethod]
    public void TestMethod5() {
        NumberNode<int> expect = new NumberNode<int>(5);
        Assert.AreEqual(expect, CreateSyntaxTree("5"));
    }

    [TestMethod]
    public void TestMethod6() {
        OperatorNode expect = new OperatorNode(
            OperatorType.Xor,
            new NumberNode<int>(4),
            new NumberNode<int>(7));
        Assert.AreEqual(expect, CreateSyntaxTree("4 ^ 7"));
    }

    [TestMethod]
    public void TestMethod7() {
        OperatorNode expect = new OperatorNode(
            OperatorType.Xor,
            new NumberNode<int>(4),
            new OperatorNode(
                OperatorType.Plus,
                new NumberNode<int>(7),
                new NumberNode<int>(2)));
        Assert.AreEqual(expect, CreateSyntaxTree("4 ^ (7 + 2)"));
    }

    private static INode CreateSyntaxTree(string expression) {
        Lexer lexer = new Lexer(expression);
        Parser<int> parser = new Parser<int>(lexer);
        INode syntaxTree = parser.Parse();
        return syntaxTree;
    }
}