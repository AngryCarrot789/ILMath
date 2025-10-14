namespace ILMath.SyntaxTree;

/// <summary>
/// Represents a unary operation node
/// </summary>
public class UnaryNode : INode {
    /// <summary>
    /// Gets the operator
    /// </summary>
    public OperatorType Operator { get; }
    
    /// <summary>
    /// Gets the single node that this node operates upon
    /// </summary>
    public INode Child { get; }

    int INode.ChildrenCount => 1;

    public UnaryNode(OperatorType @operator, INode child) {
        this.Operator = @operator;
        this.Child = child;
    }

    void INode.GetChildNodes(Span<INode> nodes) {
        nodes[0] = this.Child;
    }

    public override string ToString() {
        return $"({this.Operator.ToToken()}{this.Child})";
    }

    public override int GetHashCode() {
        HashCode hash = new HashCode();
        hash.Add(this.Operator);
        hash.Add(this.Child);
        return hash.ToHashCode();
    }

    public override bool Equals(object? obj) {
        return obj is UnaryNode other && this.Operator == other.Operator && this.Child.Equals(other.Child);
    }
}