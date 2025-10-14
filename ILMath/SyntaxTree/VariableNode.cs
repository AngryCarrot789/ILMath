namespace ILMath.SyntaxTree;

public class VariableNode : INode {
    public string Identifier { get; }

    public VariableNode(string identifier) {
        this.Identifier = identifier;
    }

    public IEnumerable<INode> EnumerateChildren() {
        yield break;
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