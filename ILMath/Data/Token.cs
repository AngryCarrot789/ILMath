namespace ILMath.Data;

/// <summary>
/// Represents a token.
/// </summary>
public readonly struct Token {
    /// <summary>
    /// The type of this token.
    /// </summary>
    public TokenType Type { get; } = TokenType.None;

    /// <summary>
    /// The value of the current token. Might be null.
    /// </summary>
    public string? Value { get; } = null;

    public Token(TokenType type, string? value) {
        this.Type = type;
        this.Value = value;
    }

    public Token(TokenType type) {
        this.Type = type;
    }

    public override string ToString() {
        return string.IsNullOrWhiteSpace(this.Value) ? $"Token({this.Type})" : $"Token({this.Type}, {this.Value})";
    }
}