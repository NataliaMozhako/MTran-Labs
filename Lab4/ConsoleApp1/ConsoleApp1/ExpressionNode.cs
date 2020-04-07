using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApp1.Token;

namespace ConsoleApp1
{
    class ExpressionNode
    {
        public ExpressionNode Left = null;
        public Token Operator = null;
        public ExpressionTypes Type;
        public int OperatorPriority = 0;
        public ExpressionNode Right = null;
        public ExpressionNode Parent = null;
        public SemanticTreeList Block = new SemanticTreeList(null);

        public ExpressionNode LeftRotation()
        {
            ExpressionNode newRoot = new ExpressionNode()
            {
                Right = this.Right.Right,
                Operator = this.Right.Operator,
                Type = this.Right.Type,
                Parent = this.Parent
            };
            newRoot.Left = new ExpressionNode()
            {
                Left = this.Left,
                Right = this.Right.Left,
                Operator = this.Operator,
                Type = this.Type,
                Parent = newRoot
            };

            return newRoot;
        }

        public bool IsLeaf
        {
            get => Left == null && Right == null;
        }

        public void InsertDeepLeft(ExpressionNode node)
        {
            ExpressionNode temp = this;
            while (!(temp.Left is null))
            {
                temp = temp.Left;
            }
            temp.Left = node;
        }

        public override string ToString()
        {
            return $"({Operator.ToString()})";
        }

        public enum ExpressionTypes
        {
            UNKNOWN,
            SINGLE_OPERATION,
            UNARY_OPERATION,
            BINARY_OPERATION,
            BLOCK_OPENING_CONDITIONAL_OPERATION,
            BLOCK_OPENING_OPERATION,
            FUNCTION_CALL,
            INDEXER_CALL,
            FUNCTION_DEF,
            COLON,
            OPERAND
        };

        public static Dictionary<TokenTypes, ExpressionTypes> TokensToExpressionTypes = new Dictionary<TokenTypes, ExpressionTypes>()
        {
            [TokenTypes.ASSIGN] = ExpressionTypes.BINARY_OPERATION,
            [TokenTypes.COMMA] = ExpressionTypes.BINARY_OPERATION,
            [TokenTypes.DOT] = ExpressionTypes.BINARY_OPERATION,
            [TokenTypes.IF] = ExpressionTypes.BLOCK_OPENING_CONDITIONAL_OPERATION,
            [TokenTypes.ELIF] = ExpressionTypes.BLOCK_OPENING_CONDITIONAL_OPERATION,
            [TokenTypes.ELSE] = ExpressionTypes.BLOCK_OPENING_OPERATION,
            [TokenTypes.FOR] = ExpressionTypes.BLOCK_OPENING_CONDITIONAL_OPERATION,
            [TokenTypes.WHILE] = ExpressionTypes.BLOCK_OPENING_CONDITIONAL_OPERATION,
            [TokenTypes.PLUS] = ExpressionTypes.BINARY_OPERATION,
            [TokenTypes.MINUS] = ExpressionTypes.BINARY_OPERATION,
            [TokenTypes.MODULE] = ExpressionTypes.BINARY_OPERATION,
            [TokenTypes.DIVISION] = ExpressionTypes.BINARY_OPERATION,
            [TokenTypes.MULTIPLICATION] = ExpressionTypes.BINARY_OPERATION,
            [TokenTypes.NOT] = ExpressionTypes.UNARY_OPERATION,
            [TokenTypes.AND] = ExpressionTypes.BINARY_OPERATION,
            [TokenTypes.OR] = ExpressionTypes.BINARY_OPERATION,
            [TokenTypes.IN] = ExpressionTypes.BINARY_OPERATION,
            [TokenTypes.LOWER] = ExpressionTypes.BINARY_OPERATION,
            [TokenTypes.LOWER_OR_EQUAL] = ExpressionTypes.BINARY_OPERATION,
            [TokenTypes.GREATER] = ExpressionTypes.BINARY_OPERATION,
            [TokenTypes.GREATER_OR_EQUAL] = ExpressionTypes.BINARY_OPERATION,
            [TokenTypes.NOT_EQUAL] = ExpressionTypes.BINARY_OPERATION,
            [TokenTypes.EQUAL] = ExpressionTypes.BINARY_OPERATION,
            [TokenTypes.FUNCTION_DEFINITION] = ExpressionTypes.FUNCTION_DEF,

            [TokenTypes.STRING_CONST] = ExpressionTypes.OPERAND,
            [TokenTypes.INT_NUM] = ExpressionTypes.OPERAND,
            [TokenTypes.FLOAT_NUM] = ExpressionTypes.OPERAND,
            [TokenTypes.ID] = ExpressionTypes.OPERAND,
            [TokenTypes.BUILT_IN_FUNCTION] = ExpressionTypes.OPERAND,
            [TokenTypes.COLON] = ExpressionTypes.COLON,
            [TokenTypes.BREAK] = ExpressionTypes.SINGLE_OPERATION,
            [TokenTypes.CONTINUE] = ExpressionTypes.SINGLE_OPERATION
        };
    }
}