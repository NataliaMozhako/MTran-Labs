using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class SemanticItem
    {
        public string Name { get; set; }
        public VarTypes VarType { get; set; }
        public FunctionSpecification FSpecification { get; set; }

        public enum VarTypes
        {
            NUMBER_VAR,
            INTEGER_VAR,
            FLOAT_VAR,
            STRING_VAR,
            FUNCTION_VAR,
            LIST_VAR,
            BOOL_VAR,
            NONE_VAR,
        }

        public static Dictionary<string, VarTypes> StringVarTypes = new Dictionary<string, VarTypes>
        {
            ["int"] = VarTypes.INTEGER_VAR,
            ["float"] = VarTypes.FLOAT_VAR,
            ["str"] = VarTypes.STRING_VAR,
            ["bool"] = VarTypes.BOOL_VAR,
            ["list"] = VarTypes.LIST_VAR,
            ["None"] = VarTypes.NONE_VAR,
            ["function"] = VarTypes.FUNCTION_VAR
        };

        public struct FunctionSpecification
        {
            public VarTypes ReturnType { get; set; }
            public int MaxArgumentsAmount { get; set; }
            public int MinArgumentsAmount { get; set; }
        }

        public static Dictionary<string, FunctionSpecification> BuiltInFunctionsReference = new Dictionary<string, FunctionSpecification>
        {
            ["print"] = new FunctionSpecification() { ReturnType = VarTypes.NONE_VAR, MinArgumentsAmount = 0, MaxArgumentsAmount = 100 },
            ["input"] = new FunctionSpecification() { ReturnType = VarTypes.STRING_VAR, MinArgumentsAmount = 0, MaxArgumentsAmount = 1 },
            ["range"] = new FunctionSpecification() { ReturnType = VarTypes.INTEGER_VAR, MinArgumentsAmount = 1, MaxArgumentsAmount = 3 },
            ["type"] = new FunctionSpecification() { ReturnType = VarTypes.STRING_VAR, MinArgumentsAmount = 1, MaxArgumentsAmount = 3 },
            ["abs"] = new FunctionSpecification() { ReturnType = VarTypes.NUMBER_VAR, MinArgumentsAmount = 1, MaxArgumentsAmount = 1 },
            ["max"] = new FunctionSpecification() { ReturnType = VarTypes.NUMBER_VAR, MinArgumentsAmount = 1, MaxArgumentsAmount = 100 },
            ["min"] = new FunctionSpecification() { ReturnType = VarTypes.NUMBER_VAR, MinArgumentsAmount = 1, MaxArgumentsAmount = 100 },
            ["int"] = new FunctionSpecification() { ReturnType = VarTypes.INTEGER_VAR, MinArgumentsAmount = 0, MaxArgumentsAmount = 1 },
            ["float"] = new FunctionSpecification() { ReturnType = VarTypes.FLOAT_VAR, MinArgumentsAmount = 0, MaxArgumentsAmount = 1 }
        };
    }
}