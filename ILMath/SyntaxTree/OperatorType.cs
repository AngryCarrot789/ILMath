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
}

public static class OperatorTypeExtensions {
    /// <summary>
    /// Converts the operator into the text that would be used to tokenize it into the enum value
    /// </summary>
    /// <param name="opType"></param>
    /// <returns></returns>
    public static string ToToken(this OperatorType opType) {
        switch (opType) {
            case OperatorType.Plus:           return "+";
            case OperatorType.Minus:          return "-";
            case OperatorType.Multiplication: return "*";
            case OperatorType.Division:       return "/";
            case OperatorType.Modulo:         return "%";
            case OperatorType.Xor:            return "^";
            case OperatorType.LShift:         return "<<";
            case OperatorType.RShift:         return ">>";
            case OperatorType.And:            return "&";
            case OperatorType.Or:             return "|";
            case OperatorType.OnesComplement: return "~";
            case OperatorType.BoolNot:        return "!";
            default:                          throw new ArgumentOutOfRangeException(nameof(opType), opType, null);
        }
    }
}