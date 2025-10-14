using System.Text;
using ILMath.Data;

namespace ILMath;

/// <summary>
/// Analyzes the input string and returns a list of tokens.
/// </summary>
public class Lexer {
    /// <summary>
    /// The current token. Consume to get the next token.
    /// </summary>
    public Token CurrentToken { get; private set; }

    private readonly string input;
    private int index;

    public Lexer(string input) {
        this.input = input;
        this.Consume(TokenType.None);
    }

    /// <summary>
    /// Consumes the current token and sets <see cref="CurrentToken"/> to the next token.
    /// </summary>
    /// <param name="type">The type </param>
    public bool Consume(TokenType type) {
        if (this.CurrentToken.Type != type)
            return false;

        this.CurrentToken = this.NextToken();
        return true;
    }

    private Token NextToken() {
        // Skip whitespace, if any
        while (this.index < this.input.Length && char.IsWhiteSpace(this.input[this.index]))
            this.index++;

        // Check if we are at the end of the input
        if (this.index >= this.input.Length)
            return new Token(TokenType.EndOfInput);

        char currentChar = this.input[this.index];
        Token token;
        switch (currentChar) {
            case '+': token = new Token(TokenType.Plus); break;
            case '-': token = new Token(TokenType.Minus); break;
            case '*': token = new Token(TokenType.Multiplication); break;
            case '/': token = new Token(TokenType.Division); break;
            case '%': token = new Token(TokenType.Modulo); break;
            case '^': token = new Token(TokenType.Xor); break;
            case '(': token = new Token(TokenType.OpenParenthesis); break;
            case ')': token = new Token(TokenType.CloseParenthesis); break;
            case ',': token = new Token(TokenType.Comma); break;
            case '&': token = new Token(TokenType.And); break;
            case '|': token = new Token(TokenType.Or); break;
            case '~': token = new Token(TokenType.OnesComplement); break;
            case '!': token = new Token(TokenType.BoolNot); break;
            case '<': token = this.NextMultiCharToken('<', TokenType.LShift); break;
            case '>': token = this.NextMultiCharToken('>', TokenType.RShift); break;
            case '=': token = this.NextMultiCharToken('=', TokenType.Equals); break;
            default:  token = new Token(TokenType.None); break;
        }

        // If we found a symbol, return it
        if (token.Type != TokenType.None) {
            this.index++;
            return token;
        }

        // Else, we found a number or identifier
        return this.NextNonSymbolToken();
    }

    private Token NextMultiCharToken(char nextChar, TokenType tokenType) {
        this.index++;
        return this.index < this.input.Length && this.input[this.index] == nextChar
            ? new Token(tokenType)
            : new Token(TokenType.Unknown);
    }

    private Token NextNonSymbolToken() {
        char currentChar = this.input[this.index];

        // If it is a number, read the whole number
        if (char.IsDigit(currentChar))
            return this.NextNumber();

        // If it is a letter, read the whole identifier
        if (IsIdentifierChar(currentChar))
            return this.NextIdentifier();

        // Else, we found an unknown token
        this.index++;
        return new Token(TokenType.Unknown, currentChar.ToString());
    }

    private Token NextIdentifier() {
        StringBuilder builder = new StringBuilder();

        // Read the whole identifier
        while (this.index < this.input.Length && IsIdentifierChar(this.input[this.index])) {
            builder.Append(this.input[this.index]);
            this.index++;
        }

        return new Token(TokenType.Identifier, builder.ToString());
    }

    private static bool IsIdentifierChar(char c) {
        switch (c) {
            case '_':
            case '@':
            case '$':
            case '#':
                return true;
            default:  return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || char.IsDigit(c);
        }
    }

    private Token NextNumber() {
        StringBuilder builder = new StringBuilder();

        // Read the whole number
        while (this.index < this.input.Length && char.IsDigit(this.input[this.index])) {
            builder.Append(this.input[this.index]);
            this.index++;
        }

        // If we found a dot, read the decimal part
        if (this.index < this.input.Length && this.input[this.index] == '.') {
            builder.Append(this.input[this.index]);
            this.index++;

            while (this.index < this.input.Length && char.IsDigit(this.input[this.index])) {
                builder.Append(this.input[this.index]);
                this.index++;
            }
        }

        return new Token(TokenType.Number, builder.ToString());
    }
}