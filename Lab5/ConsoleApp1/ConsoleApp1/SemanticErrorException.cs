using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static ConsoleApp1.SemanticItem;

namespace ConsoleApp1
{
    class SemanticErrorException: FormatException
    {
        static Regex functionArguementsRegex = new Regex(
               @"^\s*(\w+)[(][)] takes (\d+) positional argument[s]* but (\d+)",
               RegexOptions.Compiled | RegexOptions.IgnoreCase
            );

        static Regex functionWithoutArgsRegex = new Regex(
            @"^\s*(\w+)[(][)] missing (\d+)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
            );
        
        static Regex binaryOperationArgumentsRegex = new Regex(
               @" ^\s*unsupported operand type[(]s[)] for ([+\-\/*<=>!%(){},\[\]]+)[:] ['](\w+)['] and ['](\w+)[']",
               RegexOptions.Compiled | RegexOptions.IgnoreCase
            );
        static Regex nameErrorRegex = new Regex(
            @"^\s*name ['](\w+)['].+",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
            );

        public string Value { get; set; }
        public int PositionInLine { get; set; }
        public int LineNumber { get; set; }
        public string ErrorType { get; set; }
        public string Description { get; set; }

        public SemanticErrorException(string errorType, int lineNumber, string value) : base(errorType)
        {
            Value = value;
            PositionInLine = 0;
            LineNumber = lineNumber;
            ErrorType = errorType;
        }

        public SemanticErrorException(string errorType, string description, int lineNumber, string value) : this(errorType, lineNumber, value)
        {
            Description = description;
            prepareErrorDescription();
        }

        protected void prepareErrorDescription()
        {
            switch (ErrorType)
            {
                case "ZeroDivisionError":
                    PositionInLine = Value.LastIndexOf("/");
                    break;
                case "TypeError":
                    {
                        Match noArgsMatch = functionWithoutArgsRegex.Match(Description);
                        if (noArgsMatch.Success)
                        {
                            string functionName = noArgsMatch.Groups[1].Value;
                            int argsRequired = int.Parse(noArgsMatch.Groups[2].Value);
                            Description = $"<{functionName}> FUNCTION too less args (required {argsRequired} more)";
                            break;
                        }
                        Match functionErrorMatch = functionArguementsRegex.Match(Description);
                        if (functionErrorMatch.Success)
                        {
                            int argsRequired = int.Parse(functionErrorMatch.Groups[2].Value);
                            int argsGiven = int.Parse(functionErrorMatch.Groups[3].Value);
                            
                            Description = $"<{functionErrorMatch.Groups[1].Value}> FUNCTION too many args (takes {argsRequired} args only)";
                            break;
                        }
                        Match binaryOperationErrorMatch = binaryOperationArgumentsRegex.Match(Description);
                        if (binaryOperationErrorMatch.Success)
                        {
                            string operation = binaryOperationErrorMatch.Groups[1].Value;
                            string type1 = binaryOperationErrorMatch.Groups[2].Value;
                            string type2 = binaryOperationErrorMatch.Groups[3].Value;
                            PositionInLine = Value.LastIndexOf(operation);
                            operation = Token.GetTokenType(operation).ToString();
                            VarTypes varType = VarTypes.NONE_VAR;
                            if (StringVarTypes.TryGetValue(type1, out varType))
                                type1 = varType.ToString();
                            if (StringVarTypes.TryGetValue(type2, out varType))
                                type2 = varType.ToString();
                            Description = $"operation << {type1} {operation} {type2} >> is impossible";
                            break;
                        }
                        break;
                    }
                case "NameError":
                    {
                        Match nameErrorMatch = nameErrorRegex.Match(Description);
                        if (nameErrorMatch.Success)
                        {
                            string name = nameErrorMatch.Groups[1].Value;
                            Description = $"<<{name}>> UNKNOWN_VAR";
                            PositionInLine = Value.IndexOf(name);
                        }
                        break;
                    }
                default:
                    break;
            }
        }
    }
}
