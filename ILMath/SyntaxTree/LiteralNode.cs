using System.Numerics;

namespace ILMath.SyntaxTree;

public class LiteralNode<T> : INode where T : unmanaged, INumber<T> {
    /// <summary>
    /// Gets the numeric value
    /// </summary>
    public T Value { get; }

    int INode.ChildrenCount => 0;

    public LiteralNode(T value) {
        this.Value = value;
    }

    void INode.GetChildNodes(Span<INode> nodes) {
    }

    public override string ToString() {
        return this.Value.ToString() ?? "";
    }

    public override int GetHashCode() {
        return this.Value.GetHashCode();
    }

    public override bool Equals(object? obj) {
        return obj is LiteralNode<T> other && this.Value.Equals(other.Value);
    }
}