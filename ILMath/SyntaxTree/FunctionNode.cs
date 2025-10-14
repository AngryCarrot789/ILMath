namespace ILMath.SyntaxTree;

/// <summary>
/// Represents a function call node, containing any number of parameter nodes
/// </summary>
public class FunctionNode : INode {
    private readonly List<INode> myNodeList;
    
    /// <summary>
    /// The function name
    /// </summary>
    public string Identifier { get; }

    /// <summary>
    /// The parameters of the function
    /// </summary>
    public IReadOnlyList<INode> Parameters => this.myNodeList;

    int INode.ChildrenCount => this.Parameters.Count;

    public FunctionNode(string identifier, IEnumerable<INode> parameters) {
        this.Identifier = identifier;
        this.myNodeList = parameters.ToList();
    }

    void INode.GetChildNodes(Span<INode> nodes) {
        this.myNodeList.CopyTo(nodes);
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
        if (obj is FunctionNode other) {
            if (this.Identifier != other.Identifier)
                return false;
            if (this.Parameters.Count != other.Parameters.Count)
                return false;
            return this.Parameters.SequenceEqual(other.Parameters, EqualityComparer<INode>.Default);
        }

        return false;
    }
}