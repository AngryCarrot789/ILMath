namespace ILMath.SyntaxTree;

/// <summary>
/// Represents a node that operates on a left and right value
/// </summary>
public class OperatorNode : INode {
    /// <summary>
    /// Gets the operator type
    /// </summary>
    public OperatorType Operator { get; }

    /// <summary>
    /// Gets the left expression
    /// </summary>
    public INode Left { get; }

    /// <summary>
    /// Gets the right expression
    /// </summary>
    public INode Right { get; }

    int INode.ChildrenCount => 2;

    public OperatorNode(OperatorType @operator, INode left, INode right) {
        this.Operator = @operator;
        this.Left = left;
        this.Right = right;
    }

    void INode.GetChildNodes(Span<INode> nodes) {
        nodes[0] = this.Left;
        nodes[1] = this.Right;
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