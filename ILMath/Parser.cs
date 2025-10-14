using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using ILMath.Compiler;
using ILMath.Data;
using ILMath.Exception;
using ILMath.SyntaxTree;

namespace ILMath;

/// <summary>
/// Parses the tokens and returns an expression tree.
/// </summary>
public class Parser<T> where T : unmanaged, INumber<T> {
    private readonly Lexer lexer;
    private readonly ParsingContext ctx;

    public Parser(Lexer lexer) : this(lexer, default) {
    }
    
    public Parser(Lexer lexer, ParsingContext parsingContext) {
        this.lexer = lexer;
        this.ctx = parsingContext;
    }

    public INode Parse() {
        INode node = this.ParsePrecedence(0);
        this.Consume(TokenType.EndOfInput);
        return node;
    }

    private INode ParsePrecedence(int minPrecedence) {
        INode left = this.ParsePrimary();
        while (ParserUtils.Operators.TryGetValue(this.lexer.CurrentToken.Type, out (int precedence, bool right, OperatorType opType) opInfo) && opInfo.precedence >= minPrecedence) {
            this.Consume(this.lexer.CurrentToken.Type);
            left = new OperatorNode(opInfo.opType, left, this.ParsePrecedence(opInfo.right ? opInfo.precedence : (opInfo.precedence + 1)));
        }

        return left;
    }

    private INode ParsePrimary() {
        Token currentToken = this.lexer.CurrentToken;
        switch (currentToken.Type) {
            case TokenType.Literal:
                this.Consume(TokenType.Literal);
                Debug.Assert(currentToken.Value != null);
                return new LiteralNode<T>(this.ParseLiteral(currentToken));

            case TokenType.Identifier:
                string name = currentToken.Value!;
                this.Consume(TokenType.Identifier);
                if (this.lexer.CurrentToken.Type == TokenType.OpenParenthesis) {
                    this.Consume(TokenType.OpenParenthesis);
                    List<INode> args = this.ParseParameters().ToList();
                    this.Consume(TokenType.CloseParenthesis);
                    return new FunctionNode(name, args);
                }

                return new VariableNode(name);

            case TokenType.OpenParenthesis:
                this.Consume(TokenType.OpenParenthesis);
                INode expr = this.ParsePrecedence(0);
                this.Consume(TokenType.CloseParenthesis);
                return expr;

            case TokenType.Plus:
            case TokenType.Minus:
            case TokenType.OnesComplement:
            case TokenType.BoolNot:
                OperatorType unaryOp = currentToken.Type switch {
                    TokenType.Plus => OperatorType.Plus,
                    TokenType.Minus => OperatorType.Minus,
                    TokenType.OnesComplement => OperatorType.OnesComplement,
                    TokenType.BoolNot => OperatorType.BoolNot,
                    _ => throw new ParserException("Unexpected unary operator")
                };

                this.Consume(currentToken.Type);
                return new UnaryNode(unaryOp, this.ParsePrecedence(ParserUtils.UnaryPrecedence));

            default: throw new ParserException($"Unexpected token: {currentToken}");
        }
    }

    private T ParseLiteral(Token currentToken) {
        string value = currentToken.Value ?? throw new ParserException("Token has no value");
        if (!Util<T>.IsFP) {
            ParserUtils.TryGetPrefix(value, out NumberStyles style, out int endOfPrefix);
            if (endOfPrefix == -1 && this.ctx.DefaultIntegerParseMode == IntegerParseMode.Integer) {
                // Token has no prefix and the parsing context says parse as normal number
                return T.Parse(value, null);
            }

            if (endOfPrefix == -1) {
                // Token has no prefix but the parsing context specifies a default parsing format
                style = this.ctx.DefaultIntegerParseMode == IntegerParseMode.Hexadecimal ? NumberStyles.HexNumber : NumberStyles.BinaryNumber;
                endOfPrefix = 0;
            }

            Debug.Assert(style == NumberStyles.HexNumber || style == NumberStyles.BinaryNumber);
            return T.Parse(value.AsSpan(endOfPrefix), style, null);
        }
        
        return T.Parse(value, null);
    }

    private IEnumerable<INode> ParseParameters() {
        if (this.lexer.CurrentToken.Type == TokenType.CloseParenthesis)
            yield break;

        yield return this.ParsePrecedence(0);
        while (this.lexer.CurrentToken.Type == TokenType.Comma) {
            this.Consume(TokenType.Comma);
            yield return this.ParsePrecedence(0);
        }
    }

    private void Consume(TokenType type) {
        if (!this.lexer.Consume(type))
            throw new ParserException($"Unexpected token: {this.lexer.CurrentToken} (expected {type})");
    }
}

internal static class ParserUtils {
    public static readonly Dictionary<TokenType, (int precedence, bool right, OperatorType opType)> Operators;
    public const int UnaryPrecedence = 7;

    static ParserUtils() {
        Operators = new Dictionary<TokenType, (int precedence, bool right, OperatorType opType)> {
            { TokenType.Or, (1, false, OperatorType.Or) },
            { TokenType.Xor, (2, false, OperatorType.Xor) },
            { TokenType.And, (3, false, OperatorType.And) },
            { TokenType.LShift, (4, false, OperatorType.LShift) },
            { TokenType.RShift, (4, false, OperatorType.RShift) },
            { TokenType.Plus, (5, false, OperatorType.Plus) },
            { TokenType.Minus, (5, false, OperatorType.Minus) },
            { TokenType.Multiplication, (6, false, OperatorType.Multiplication) },
            { TokenType.Division, (6, false, OperatorType.Division) },
            { TokenType.Modulo, (6, false, OperatorType.Modulo) },
        };
    }

    public static void TryGetPrefix(string input, out NumberStyles numberStyles, out int endOfPrefix) {
        int j, i = input.IndexOf("0x", StringComparison.OrdinalIgnoreCase);
        if (i != -1) {
            numberStyles = NumberStyles.HexNumber;
            while ((j = input.IndexOf("0x", i + 2, StringComparison.OrdinalIgnoreCase)) != -1)
                i = j;
            endOfPrefix = i + 2;
            return;
        }

        if ((i = input.IndexOf("0b", StringComparison.OrdinalIgnoreCase)) != -1) {
            numberStyles = NumberStyles.BinaryNumber;
            while ((j = input.IndexOf("0b", i + 2, StringComparison.OrdinalIgnoreCase)) != -1)
                i = j;
            endOfPrefix = i + 2;
            return;
        }

        numberStyles = default;
        endOfPrefix = -1;
    }
}