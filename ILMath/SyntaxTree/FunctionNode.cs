using System.Diagnostics;

namespace ILMath.SyntaxTree;

public class FunctionNode : INode {
    public string Identifier { get; }
    public IReadOnlyList<INode> Parameters { get; }

    public FunctionNode(string identifier, IEnumerable<INode> parameters) {
        this.Identifier = identifier;
        this.Parameters = parameters.ToList();
    }

    public IEnumerable<INode> EnumerateChildren() {
        return this.Parameters;
    }

    public override string ToString() {
        return $"{this.Identifier}({string.Join(", ", this.Parameters)}))";
    }

    public override int GetHashCode() {
        HashCode hash = new HashCode();
        hash.Add(this.Identifier);
        foreach (INode parameter in this.Parameters)
            hash.Add(parameter);
        return hash.ToHashCode();
    }

    public override bool Equals(object? obj) {
        if (obj is not FunctionNode other)
            return false;
        if (this.Identifier != other.Identifier)
            return false;
        if (this.Parameters.Count != other.Parameters.Count)
            return false;
        bool equals = true;
        for (int i = 0; i < this.Parameters.Count; i++)
            equals &= this.Parameters[i].Equals(other.Parameters[i]);
        return equals;
    }
}