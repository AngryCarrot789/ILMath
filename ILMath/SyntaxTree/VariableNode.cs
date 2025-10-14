namespace ILMath.SyntaxTree;

/// <summary>
/// Represents a node that accesses a node to get a value
/// </summary>
public class VariableNode : INode {
    public string Identifier { get; }

    int INode.ChildrenCount => 0;
    
    public VariableNode(string identifier) {
        this.Identifier = identifier;
    }

    void INode.GetChildNodes(Span<INode> nodes) {
    }

    public override string ToString() {
        return $"{this.Identifier}";
    }

    public override int GetHashCode() {
        return this.Identifier.GetHashCode();
    }

    public override bool Equals(object? obj) {
        if (obj is not VariableNode other)
            return false;
        return this.Identifier.Equals(other.Identifier);
    }
}