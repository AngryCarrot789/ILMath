namespace ILMath.SyntaxTree;

public class UnaryNode : INode {
    public OperatorType Operator { get; }
    public INode Child { get; }

    public UnaryNode(OperatorType @operator, INode child) {
        this.Operator = @operator;
        this.Child = child;
    }

    public IEnumerable<INode> EnumerateChildren() {
        yield return this.Child;
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
        if (obj is not UnaryNode other)
            return false;
        return this.Operator == other.Operator && this.Child.Equals(other.Child);
    }
}