using System.Numerics;

namespace ILMath.SyntaxTree;

public class NumberNode<T> : INode where T : unmanaged, INumber<T> {
    public T Value { get; }

    public NumberNode(T value) {
        this.Value = value;
    }

    public IEnumerable<INode> EnumerateChildren() {
        yield break;
    }

    public override string ToString() {
        return this.Value.ToString() ?? "";
    }

    public override int GetHashCode() {
        return this.Value.GetHashCode();
    }

    public override bool Equals(object? obj) {
        if (obj is not NumberNode<T> other)
            return false;
        return this.Value.Equals(other.Value);
    }
}