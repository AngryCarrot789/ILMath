using System.Diagnostics;

namespace ILMath.SyntaxTree;

public class OperatorNode : INode {
    public OperatorType Operator { get; }
    public INode Left { get; }
    public INode Right { get; }

    public OperatorNode(OperatorType @operator, INode left, INode right) {
        this.Operator = @operator;
        this.Left = left;
        this.Right = right;
    }

    public IEnumerable<INode> EnumerateChildren() {
        yield return this.Left;
        yield return this.Right;
    }

    public override string ToString() {
        return $"({this.Left} {this.Operator.ToToken()} {this.Right})";
    }

    public override int GetHashCode() {
        HashCode hash = new HashCode();
        hash.Add(this.Operator);
        hash.Add(this.Left);
        hash.Add(this.Right);
        return hash.ToHashCode();
    }

    public override bool Equals(object? obj) {
        if (obj is not OperatorNode other)
            return false;
        return this.Operator == other.Operator && this.Left.Equals(other.Left) && this.Right.Equals(other.Right);
    }
}