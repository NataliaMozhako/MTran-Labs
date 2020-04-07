using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static ConsoleApp1.Token;

namespace ConsoleApp1
{
    public class LexicalAnalizer
    {
        private List<Token> _tokens = new List<Token>();
        private List<Construction> _constructions = new List<Construction>();
        private List<LexicalError> _error = new List<LexicalError>();

        private const string COMMENT_REGEX_GROUP = "Comment";
        private const string STRING_REGEX_GROUP = "String";
        private const string FLOAT_REGEX_GROUP = "Float";
        private const string INTEGER_REGEX_GROUP = "Integer";
        private const string ID_REGEX_GROUP = "ID";
        private const string OPERATOR_REGEX_GROUP = "Operator";

        private const string OTHER_REGEX_GROUP = "Other";

        private Regex _regex = new Regex(
                @"\s*(?:(?<Comment>#.*)|(?<String>[\""'].*[\""'])"
                    + @"|(?<Float>[+-]*[0-9]+\.[0-9]*)|(?<Integer>[+-]*\d+)"
                    + @"|(?<Operator>[+\-\/*<=>!%(){},\[\]:]+)"
                    + @"|(?<ID>\w+)|(?<Other>.+\s?))",
                RegexOptions.Compiled | RegexOptions.IgnoreCase
                );


        public void AnaliseLines(IEnumerable<string> codeLines)
        {
            int lineNumber = 0;
            foreach (string line in codeLines)
            {
                var analiseResult = AnaliseLine(line, lineNumber);
                _constructions.Add(analiseResult);
                lineNumber++;
            }
        }

        public Construction AnaliseLine(string codeLine, int lineNumber)
        {
            var trimedLine = codeLine.TrimStart('\t');
            int spaces = codeLine.Length - trimedLine.Length;
            var (tokens, errors) = ParseLine(trimedLine, lineNumber);

            // if lexemes[0] == "for" && lexemes[2] == "in" => return For

            return new Construction()
            {
                Tokens = tokens,
                Errors = errors,
                Indentation = spaces
            };
        }

        public (List<Token> tokens, List<LexicalError> errors) ParseLine(string codeLine, int lineNumber)
        {
            List<Token> tokens = new List<Token>();
            List<LexicalError> errors = new List<LexicalError>();
            MatchCollection matches = _regex.Matches(codeLine);
            string[] groupNames = _regex.GetGroupNames();

            foreach (Match match in matches)
            {
                GroupCollection groups = match.Groups;
                for (int i = 1; i < groupNames.Length; i++)
                {
                    if (groups[groupNames[i]].Success)
                    {
                        string trimmedValue = groups[groupNames[i]].Value.Trim(' ');
                        if (trimmedValue.Length == 0)
                            break;

                        TokenTypes type = GetTokenType(groupNames[i], trimmedValue);
                        // possible errors are collected, but tokens are still inserted into tokens list
                        if (type == TokenTypes.UNKNOWN)
                        {
                            LexicalError error = new LexicalError()
                            {
                                CodeLineNumber = lineNumber,
                                Value = groups[groupNames[i]].Value,
                                IndexInCodeLine = match.Index,
                                Length = match.Length
                            };
                            error.CreateAndSetDescription(codeLine);
                            errors.Add(error);
                        }

                        tokens.Add(
                            new Token
                            {
                                Value = groups[groupNames[i]].Value,
                                Group = groupNames[i],
                                CodeLineNumber = lineNumber,
                                CodeLineIndex = match.Index,
                                Length = match.Length,
                                TokenType = type
                            }
                            );
                    }
                }
            }
            return (tokens, errors);
        }

        public static TokenTypes GetTokenType(string matchGroup, string value)
        {
            switch (matchGroup)
            {
                case COMMENT_REGEX_GROUP:
                    return TokenTypes.COMMENT;
                case STRING_REGEX_GROUP:
                    return TokenTypes.STRING_CONST;
                case FLOAT_REGEX_GROUP:
                    return TokenTypes.FLOAT_NUM;
                case INTEGER_REGEX_GROUP:
                    return TokenTypes.INT_NUM;
                case ID_REGEX_GROUP:
                    return GetIDTokenType(value);
                case OPERATOR_REGEX_GROUP:
                    return GetOperatorTokenType(value);

                case OTHER_REGEX_GROUP:
                default:
                    return TokenTypes.UNKNOWN;
            }
        }

        public static TokenTypes GetOperatorTokenType(string value)
        {
            TokenTypes tokenType;
            if (SimpleOperators.TryGetValue(value, out tokenType) || BlockOpeningOperators.TryGetValue(value, out tokenType))
            {
                return tokenType;
            }
            return TokenTypes.UNKNOWN;
        }

        public static TokenTypes GetIDTokenType(string value)
        {
            TokenTypes tokenType;
            if (ReservedIDs.TryGetValue(value, out tokenType) 
                || SimpleOperators.TryGetValue(value, out tokenType)
                || BlockOpeningOperators.TryGetValue(value, out tokenType))
            {
                return tokenType;
            }
            return TokenTypes.ID;
        }

        public class LexicalError
        {
            public enum ErrorTypes { UNEXPECTED_TOKEN, UNDEFINED_FUNCTION }

            public ErrorTypes ErrorType { get; set; }

            public string Value { get; set; }

            public int CodeLineNumber { get; set; }

            public int IndexInCodeLine { get; set; }

            public int Length { get; set; }

            protected string _decription = "Unexpected token";

            public string Description => _decription;

            public void CreateAndSetDescription(string codeLine)
            {
                StringBuilder stringBuilder = new StringBuilder(codeLine);
                stringBuilder.AppendLine();
                stringBuilder.Append(new string(' ', this.IndexInCodeLine));
                stringBuilder.Append('^');
                this._decription = stringBuilder.ToString();
            }
        }

        public class Construction
        {
            public List<Token> Tokens { get; set; }
            public List<LexicalError> Errors { get; set; }
            public bool HasErrors { get => Errors.Count > 0; }
            public int Indentation { get; set; } 
        }
    }
}
