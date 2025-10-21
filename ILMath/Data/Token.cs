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
        string? tokenText;
        switch (this.Type) {
            case TokenType.None:                 tokenText = null; break;
            case TokenType.Plus:                 tokenText = "+"; break;
            case TokenType.Minus:                tokenText = "-"; break;
            case TokenType.Multiplication:       tokenText = "*"; break;
            case TokenType.Division:             tokenText = "/"; break;
            case TokenType.Modulo:               tokenText = "%"; break;
            case TokenType.Xor:                  tokenText = "^"; break;
            case TokenType.OpenParenthesis:      tokenText = "("; break;
            case TokenType.CloseParenthesis:     tokenText = ")"; break;
            case TokenType.Comma:                tokenText = ","; break;
            case TokenType.LShift:               tokenText = "<<"; break;
            case TokenType.RShift:               tokenText = ">>"; break;
            case TokenType.And:                  tokenText = "&"; break;
            case TokenType.Or:                   tokenText = "|"; break;
            case TokenType.OnesComplement:       tokenText = "~"; break;
            case TokenType.BoolNot:              tokenText = "!"; break;
            case TokenType.EqualTo:              tokenText = "=="; break;
            case TokenType.NotEqualTo:           tokenText = "!="; break;
            case TokenType.LessThan:             tokenText = "<"; break;
            case TokenType.LessThanOrEqualTo:    tokenText = "<="; break;
            case TokenType.GreaterThan:          tokenText = ">"; break;
            case TokenType.GreaterThanOrEqualTo: tokenText = ">="; break;
            case TokenType.ConditionalAnd:       tokenText = "&&"; break;
            case TokenType.ConditionalOr:        tokenText = "||"; break;
            case TokenType.Identifier:
            case TokenType.Literal:
            case TokenType.EndOfInput:
            case TokenType.Unknown:
                tokenText = null;
                break;
            default: throw new ArgumentOutOfRangeException();
        }

        return string.IsNullOrWhiteSpace(this.Value) ? tokenText ?? this.Type.ToString() : $"{tokenText ?? this.Type.ToString()} ({this.Value})";
    }
}