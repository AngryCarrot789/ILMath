namespace ILMath;

/// <summary>
/// Context for parsing an equation with a <see cref="Parser{T}"/>
/// </summary>
public struct ParsingContext {
    /// <summary>
    /// Gets or sets whether we should parse integers as hexadecimal by default, rather than requiring a hex prefix
    /// </summary>
    public IntegerParseMode DefaultIntegerParseMode { get; set; }

    public ParsingContext() {
    }
}

public enum IntegerParseMode {
    /// <summary>
    /// Parse token as a normal integer
    /// </summary>
    Integer,

    /// <summary>
    /// Parse token as hexadecimal
    /// </summary>
    Hexadecimal,

    /// <summary>
    /// Parse token as binary
    /// </summary>
    Binary
}