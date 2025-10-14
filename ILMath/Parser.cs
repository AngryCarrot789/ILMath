using System.Diagnostics;
using System.Numerics;
using ILMath.Data;
using ILMath.Exception;
using ILMath.SyntaxTree;

namespace ILMath;

internal static class Parser {
    public static readonly Dictionary<TokenType, (int precedence, OperatorType opType)> Operators;

    static Parser() {
        Operators = new Dictionary<TokenType, (int precedence, OperatorType opType)> {
            { TokenType.Or,             (1, OperatorType.Or) },
            { TokenType.Xor,            (2, OperatorType.Xor) },
            { TokenType.And,            (3, OperatorType.And) },
            { TokenType.LShift,         (4, OperatorType.LShift) },
            { TokenType.RShift,         (4, OperatorType.RShift) },
            { TokenType.Plus,           (5, OperatorType.Plus) },
            { TokenType.Minus,          (5, OperatorType.Minus) },
            { TokenType.Multiplication, (6, OperatorType.Multiplication) },
            { TokenType.Division,       (6, OperatorType.Division) },
            { TokenType.Modulo,         (6, OperatorType.Modulo) }
        };
    }
}

/// <summary>
/// Parses the tokens and returns an expression tree.
/// </summary>
public class Parser<T> where T : unmanaged, INumber<T> {
    private readonly Lexer lexer;


    public Parser(Lexer lexer) {
        this.lexer = lexer;
    }

    public INode Parse() {
        INode node = this.ParsePrecedence(0);
        this.Consume(TokenType.EndOfInput);
        return node;
    }

    private INode ParsePrecedence(int minPrecedence) {
        INode left = this.ParsePrimary();
        while (Parser.Operators.TryGetValue(this.lexer.CurrentToken.Type, out (int precedence, OperatorType opType) opInfo) && opInfo.precedence >= minPrecedence) {
            this.Consume(this.lexer.CurrentToken.Type);
            left = new OperatorNode(opInfo.opType, left, this.ParsePrecedence(opInfo.precedence + 1));
        }

        return left;
    }

    private INode ParsePrimary() {
        switch (this.lexer.CurrentToken.Type) {
            case TokenType.Number:
                Token numberToken = this.lexer.CurrentToken;
                this.Consume(TokenType.Number);
                Debug.Assert(numberToken.Value != null);
                return new NumberNode<T>(T.Parse(numberToken.Value, null));

            case TokenType.Identifier:
                string name = this.lexer.CurrentToken.Value!;
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
                OperatorType unaryOp = this.lexer.CurrentToken.Type switch {
                    TokenType.Plus => OperatorType.Plus,
                    TokenType.Minus => OperatorType.Minus,
                    TokenType.OnesComplement => OperatorType.OnesComplement,
                    _ => throw new ParserException("Unexpected unary operator")
                };
                this.Consume(this.lexer.CurrentToken.Type);
                return new UnaryNode(unaryOp, this.ParsePrecedence(6)); // bind tightly

            default: throw new ParserException($"Unexpected token: {this.lexer.CurrentToken}");
        }
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