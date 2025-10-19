namespace ILMath.SyntaxTree;

public enum OperatorType {
    Plus,           // +
    Minus,          // -
    Multiplication, // *
    Division,       // /
    Modulo,         // %
    Xor,            // ^
    LShift,         // <<
    RShift,         // >>
    And,            // &
    Or,             // |
    OnesComplement, // ~
    BoolNot,        // !

    EqualTo,              // ==
    NotEqualTo,           // !=
    LessThan,             // <
    LessThanOrEqualTo,    // <=
    GreaterThan,          // >
    GreaterThanOrEqualTo, // >=
    ConditionalAnd,       // &&
    ConditionalOr,        // ||
}

public static class OperatorTypeExtensions {
    /// <summary>
    /// Converts the operator into the text that would be used to tokenize it into the enum value
    /// </summary>
    /// <param name="opType"></param>
    /// <returns></returns>
    public static string ToToken(this OperatorType opType) {
        switch (opType) {
            case OperatorType.Plus:                 return "+";
            case OperatorType.Minus:                return "-";
            case OperatorType.Multiplication:       return "*";
            case OperatorType.Division:             return "/";
            case OperatorType.Modulo:               return "%";
            case OperatorType.Xor:                  return "^";
            case OperatorType.LShift:               return "<<";
            case OperatorType.RShift:               return ">>";
            case OperatorType.And:                  return "&";
            case OperatorType.Or:                   return "|";
            case OperatorType.OnesComplement:       return "~";
            case OperatorType.BoolNot:              return "!";
            case OperatorType.EqualTo:              return "==";
            case OperatorType.NotEqualTo:           return "!=";
            case OperatorType.LessThan:             return "<";
            case OperatorType.LessThanOrEqualTo:    return "<=";
            case OperatorType.GreaterThan:          return ">";
            case OperatorType.GreaterThanOrEqualTo: return ">=";
            default:                                throw new ArgumentOutOfRangeException(nameof(opType), opType, null);
        }
    }
}