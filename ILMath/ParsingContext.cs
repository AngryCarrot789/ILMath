using System.Numerics;
using ILMath.Exception;
using ILMath.SyntaxTree;

namespace ILMath;

/// <summary>
/// Context for parsing an equation with a <see cref="Parser{T}"/>
/// </summary>
public struct ParsingContext {
    /// <summary>
    /// Gets or sets whether we should parse integers as hexadecimal by default, rather than requiring a hex prefix
    /// </summary>
    public IntegerParseMode DefaultIntegerParseMode { get; set; }

    /// <summary>
    /// Gets or sets an action that validates if a function is valid for being executed. E.g., does it exist and have the valid number of args
    /// </summary>
    public Action<FunctionNode>? ValidateFunction { get; set; }

    public ParsingContext() {
    }

    public static Action<FunctionNode> CreateFunctionValidatorForEvaluationContext<T>(IEvaluationContext<T> context) where T : unmanaged, INumber<T> {
        return n => {
            if (!context.TryGetFunction(n.Identifier, out FunctionDeclaration<T> function))
                throw new ParserException($"Unknown function: {n.Identifier}");
            
            if (function.MinParams != -1 && n.Parameters.Count < function.MinParams)
                throw new ParserException($"Not enough args. Need at least {function.MinParams}, got {n.Parameters.Count}");
            
            if (function.MaxParams != -1 && n.Parameters.Count > function.MaxParams)
                throw new ParserException($"Too many args. Maximum is {function.MaxParams}, got {n.Parameters.Count}");
        };
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