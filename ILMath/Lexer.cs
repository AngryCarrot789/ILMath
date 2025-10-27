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
    private readonly ParsingContext ctx;

    public Lexer(string input, ParsingContext parsingContext) {
        this.input = input;
        this.ctx = parsingContext;
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
            case '&': token = this.NextSingleOrMultiCharToken('&', TokenType.ConditionalAnd, TokenType.And); break;
            case '|': token = this.NextSingleOrMultiCharToken('|', TokenType.ConditionalOr, TokenType.Or); break;
            case '~': token = new Token(TokenType.OnesComplement); break;
            case '!': token = this.NextSingleOrMultiCharToken('=', TokenType.NotEqualTo, TokenType.BoolNot); break;
            case '<': token = this.NextSingleOrMultiCharToken('<', TokenType.LShift, '=', TokenType.LessThanOrEqualTo, TokenType.LessThan); break;
            case '>': token = this.NextSingleOrMultiCharToken('>', TokenType.RShift, '=', TokenType.GreaterThanOrEqualTo, TokenType.GreaterThan); break;
            case '=': token = this.NextMultiCharToken('=', TokenType.EqualTo); break;
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
        if ((this.index + 1) < this.input.Length && this.input[this.index + 1] == nextChar) {
            this.index++;
            return new Token(tokenType);
        }
        
        return new Token(TokenType.Unknown);
    }

    private Token NextSingleOrMultiCharToken(char nextChar, TokenType multiType, TokenType singleType) {
        if ((this.index + 1) < this.input.Length) {
            if (this.input[this.index + 1] == nextChar) {
                this.index++;
                return new Token(multiType);
            }

            return new Token(singleType);
        }

        return new Token(TokenType.Unknown);
    }

    private Token NextSingleOrMultiCharToken(char nch1, TokenType ntok1, char nch2, TokenType ntok2, TokenType singleType) {
        if ((this.index + 1) < this.input.Length) {
            char ch = this.input[this.index + 1];
            if (ch == nch1) {
                this.index++;
                return new Token(ntok1);
            }

            if (ch == nch2) {
                this.index++;
                return new Token(ntok2);
            }

            return new Token(singleType);
        }

        return new Token(TokenType.Unknown);
    }

    private Token NextNonSymbolToken() {
        char currentChar = this.input[this.index];

        // If it is a literal, read the whole number. Also includes when
        // a hex prefix is specified, since it starts with '0'.
        if (char.IsDigit(currentChar))
            return this.NextLiteral();

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

    private Token NextLiteral() {
        bool isHex = false;
        
        StringBuilder builder = new StringBuilder();
        char ch = this.input[this.index++];
        builder.Append(ch); // append the first literal char

        // Check if next char is the 2nd char of the hex prefix specifier. If so, add it
        if (this.index < this.input.Length && ((ch = this.input[this.index]) == 'x' || ch == 'X')) {
            builder.Append(ch);
            this.index++;
            isHex = true;
        }

        // Read the rest of the number or hex value. We use IsAsciiHexDigit because it includes 0-9 too
        while (this.index < this.input.Length && ((ch = this.input[this.index]) == '_' || char.IsAsciiHexDigit(ch))) {
            if (ch != '_') // allow separating sections of number with underscore
                builder.Append(ch);
            this.index++;
        }

        // Check for dot. Also check there's enough chars for dot and at least one decimal number
        if ((this.index + 1) < this.input.Length && (ch = this.input[this.index]) == '.' && !isHex) {
            builder.Append(ch);
            this.index++;

            while (this.index < this.input.Length && char.IsDigit(ch = this.input[this.index])) {
                builder.Append(ch);
                this.index++;
            }
        }

        return new Token(TokenType.Literal, builder.ToString());
    }

    private static bool IsIdentifierChar(char ch) {
        switch (ch) {
            case '_':
            case '@':
            case '$':
            case '#':
                return true;
            default: return (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || char.IsDigit(ch);
        }
    }
}