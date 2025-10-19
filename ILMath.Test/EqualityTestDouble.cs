using ILMath.SyntaxTree;

namespace ILMath.Test;

[TestClass]
public class EqualityTestDouble {
    [TestMethod]
    public void TestMethod1() {
        OperatorNode expect = new OperatorNode(
            OperatorType.Plus,
            new LiteralNode<double>(2.0),
            new OperatorNode(
                OperatorType.Multiplication,
                new LiteralNode<double>(1.0),
                new LiteralNode<double>(5.0)));
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
                    new LiteralNode<double>(2.0))
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
                        new LiteralNode<double>(4.0))
                ]),
            new LiteralNode<double>(8.0));
        Assert.AreEqual(expect, CreateSyntaxTree("sin(pi / 4) * 8.0"));
    }

    [TestMethod]
    public void TestMethod5() {
        LiteralNode<double> expect = new LiteralNode<double>(5.0);
        Assert.AreEqual(expect, CreateSyntaxTree("5"));
    }

    [TestMethod]
    public void TestMethod6() {
        FunctionNode expect = new FunctionNode("pow", [
            new LiteralNode<double>(4.0), new LiteralNode<double>(7.0)
        ]);

        Assert.AreEqual(expect, CreateSyntaxTree("pow(4,7)"));
    }

    [TestMethod]
    public void TestMethod7() {
        FunctionNode expect = new FunctionNode(
            "pow", [
                new LiteralNode<double>(4.0),
                new OperatorNode(
                    OperatorType.Plus,
                    new LiteralNode<double>(7.0),
                    new LiteralNode<double>(2.0))
            ]);
        Assert.AreEqual(expect, CreateSyntaxTree("pow(4, (7 + 2))"));
    }

    [TestMethod]
    public void TestMethod8() {
        FunctionNode expect = new FunctionNode("pow", [
            new LiteralNode<double>(4.0),
            new FunctionNode("pow", [
                new OperatorNode(
                    OperatorType.Plus,
                    new LiteralNode<double>(7.0),
                    new LiteralNode<double>(2.0)),
                new LiteralNode<double>(1.5)
            ])
        ]);
        
        Assert.AreEqual(expect, CreateSyntaxTree("pow(4, pow(7 + 2, 1.5))"));
    }

    private static INode CreateSyntaxTree(string expression) {
        Lexer lexer = new Lexer(expression, default);
        Parser<double> parser = new Parser<double>(lexer);
        INode syntaxTree = parser.Parse();
        return syntaxTree;
    }
}