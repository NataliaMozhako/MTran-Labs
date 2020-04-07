using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Token
    {
        public string Value { get; set; }
        public string Group { get; set; }
        public TokenTypes TokenType { get; set; }
        public int CodeLineNumber { get; set; }
        public int CodeLineIndex { get; set; }
        public int Length { get; set; }

        public bool IsReservedIdToken
        {
            get => ReservedIDs.ContainsValue(TokenType);
        }

        public bool IsOperation
        {
            get => SimpleOperators.ContainsValue(TokenType) | BlockOpeningOperators.ContainsValue(TokenType);
        }

        public bool IsBlockOpeningOperation
        {
            get => BlockOpeningOperators.ContainsValue(TokenType);
        }

        public bool IsIf
        {
            get => TokenType == TokenTypes.IF ;
        }

        public bool IsElif
        {
            get => TokenType == TokenTypes.ELIF;
        }

        public bool IsElse
        {
            get => TokenType == TokenTypes.ELSE;
        }

        public bool IsOpeningBracket
        {
            get => this.TokenType == TokenTypes.OPENING_ROUND_BRACKET || this.TokenType == TokenTypes.OPENING_SQUARE_BRACKET;
        }

        public bool IsClosingBracket
        {
            get => this.TokenType == TokenTypes.CLOSING_ROUND_BRACKET || this.TokenType == TokenTypes.CLOSING_SQUARE_BRACKET;
        }

        public bool IsConstant
        {
            get => this.TokenType == TokenTypes.STRING_CONST
                    || this.TokenType == TokenTypes.INT_NUM
                    || this.TokenType == TokenTypes.FLOAT_NUM;
        }

        public string DescriptionString
        {
            get 
            {
                if (this.IsReservedIdToken && !this.IsOperation)
                    return $"Reserved keyword {this.TokenType}";

                if (this.IsOperation)
                    return $"Operation {this.TokenType}";

                if (this.IsConstant)
                    return $"{this.TokenType} constant";

                if (this.TokenType == TokenTypes.COMMENT)
                    return $"is # comment";
                
                return $"is {this.TokenType}";
            }
        }

        public override string ToString()
        {
            return $"{TokenType}: {Value}";
        }

        public static Dictionary<string, TokenTypes> ReservedIDs = new Dictionary<string, TokenTypes>()
        {   
            ["print"] = TokenTypes.BUILT_IN_FUNCTION,
            ["input"] = TokenTypes.BUILT_IN_FUNCTION,
            ["range"] = TokenTypes.BUILT_IN_FUNCTION,
            ["type"] = TokenTypes.BUILT_IN_FUNCTION,
            ["abs"] = TokenTypes.BUILT_IN_FUNCTION,
            ["max"] = TokenTypes.BUILT_IN_FUNCTION,
            ["min"] = TokenTypes.BUILT_IN_FUNCTION,
            ["int"] = TokenTypes.BUILT_IN_FUNCTION,
            ["float"] = TokenTypes.BUILT_IN_FUNCTION,
            ["break"] = TokenTypes.BREAK,
            ["continue"] = TokenTypes.CONTINUE
        };


        public static TokenTypes GetTokenType(string value)
        {
            TokenTypes type = TokenTypes.UNKNOWN;
            if (SimpleOperators.TryGetValue(value, out type))
                return type;
            if (BlockOpeningOperators.TryGetValue(value, out type))
                return type;
            if (ReservedIDs.TryGetValue(value, out type))
                return type;
            return TokenTypes.UNKNOWN;
        }

        public static Dictionary<string, TokenTypes> SimpleOperators = new Dictionary<string, TokenTypes>()
        {
            ["="] = TokenTypes.ASSIGN,
            ["+"] = TokenTypes.PLUS,
            ["-"] = TokenTypes.MINUS,
            ["/"] = TokenTypes.DIVISION,
            ["*"] = TokenTypes.MULTIPLICATION,
            ["%"] = TokenTypes.MODULE,
            [":"] = TokenTypes.COLON,
            ["<"] = TokenTypes.LOWER,
            ["<="] = TokenTypes.LOWER_OR_EQUAL,
            [">"] = TokenTypes.GREATER,
            [">="] = TokenTypes.GREATER_OR_EQUAL,
            ["=="] = TokenTypes.EQUAL,
            ["!="] = TokenTypes.NOT_EQUAL,
            ["("] = TokenTypes.OPENING_ROUND_BRACKET,
            [")"] = TokenTypes.CLOSING_ROUND_BRACKET,
            ["["] = TokenTypes.OPENING_SQUARE_BRACKET,
            ["]"] = TokenTypes.CLOSING_SQUARE_BRACKET,
            ["{"] = TokenTypes.OPENING_CURLY_BRACKET,
            ["}"] = TokenTypes.CLOSING_CURLY_BRACKET,
            ["."] = TokenTypes.DOT,
            [","] = TokenTypes.COMMA,
        };

        public static Dictionary<string, TokenTypes> BlockOpeningOperators = new Dictionary<string, TokenTypes>()
        {
            ["and"] = TokenTypes.AND,
            ["or"] = TokenTypes.OR,
            ["not"] = TokenTypes.NOT,
            ["for"] = TokenTypes.FOR,
            ["in"] = TokenTypes.IN,
            ["while"] = TokenTypes.WHILE,
            ["if"] = TokenTypes.IF,
            ["elif"] = TokenTypes.ELIF,
            ["else"] = TokenTypes.ELSE,
            ["return"] = TokenTypes.RETURN,
            ["raise"] = TokenTypes.RAISE,
            ["import"] = TokenTypes.IMPORT,
            ["def"] = TokenTypes.FUNCTION_DEFINITION
        };

        public enum TokenTypes
        {
            UNKNOWN,
            COMMENT,
            STRING_CONST,
            FLOAT_NUM,
            INT_NUM,
            ID,
            DOT,
            COMMA,
            COLON,
            OPENING_SQUARE_BRACKET,
            CLOSING_SQUARE_BRACKET,
            OPENING_ROUND_BRACKET,
            CLOSING_ROUND_BRACKET,
            OPENING_CURLY_BRACKET,
            CLOSING_CURLY_BRACKET,
            AND,
            OR,
            NOT,
            ASSIGN,
            PLUS,
            MINUS,
            DIVISION,
            MULTIPLICATION,
            MODULE,
            GREATER,
            GREATER_OR_EQUAL,
            LOWER,
            LOWER_OR_EQUAL,
            EQUAL,
            NOT_EQUAL,
            FOR,
            IN,
            WHILE,
            IF,
            ELIF,
            ELSE,
            BUILT_IN_FUNCTION,
            RAISE,
            IMPORT,
            FUNCTION_DEFINITION,
            BREAK,
            CONTINUE,
            RETURN
        }

    }
}
