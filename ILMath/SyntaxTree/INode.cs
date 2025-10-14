namespace ILMath.SyntaxTree;

/// <summary>
/// Represents a node within a syntax tree
/// </summary>
public interface INode {
    /// <summary>
    /// Gets the number of child nodes we have
    /// </summary>
    int ChildrenCount { get; }

    /// <summary>
    /// Copy all of the child nodes into the given span
    /// </summary>
    /// <param name="nodes">The destination span</param>
    void GetChildNodes(Span<INode> nodes);
}