namespace ILMath.Data;

public enum TokenType {
    None,             // 
    Plus,             // +
    Minus,            // -
    Multiplication,   // *
    Division,         // /
    Modulo,           // %
    Xor,              // ^
    OpenParenthesis,  // (
    CloseParenthesis, // ) 
    Comma,            // ,
    
    // Binary
    LShift,           // <<
    RShift,           // >>
    And,              // &
    Or,               // |
    OnesComplement,   // ~
    Equals,           // ==
    
    Identifier,       // 
    Number,           // 
    EndOfInput,       // 
    Unknown           // 
}