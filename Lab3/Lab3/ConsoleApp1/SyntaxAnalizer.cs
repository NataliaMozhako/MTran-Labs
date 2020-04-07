using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApp1.LexicalAnalizer;
using static ConsoleApp1.Token;

namespace ConsoleApp1
{
    class SyntaxAnalizer
    {
        protected int OpenedBracketsLevel = 0;
        protected int CurrentBlockLevel = 0;

        public ExpressionNode Analyse(IEnumerable<Token> tokens, out bool startNewBlock, out bool isElifElseNode)
        {
            OpenedBracketsLevel = 0;
            startNewBlock = false;
            isElifElseNode = false;
            var firstToken = tokens.FirstOrDefault();
            
            if (firstToken?.IsBlockOpeningOperation == true)
            {
                startNewBlock = true;
                isElifElseNode = firstToken.TokenType == TokenTypes.ELSE || firstToken.TokenType == TokenTypes.ELIF;
                if (tokens.LastOrDefault()?.TokenType != Token.TokenTypes.COLON)
                {
                    var t = tokens.LastOrDefault();
                    throw new SyntaxErrorException("colon expected", t.Value, t.CodeLineIndex, t.CodeLineNumber);
                }
            }
            else if (tokens.LastOrDefault()?.TokenType == Token.TokenTypes.COLON)
            {
                throw new SyntaxErrorException("this is not a keyword", firstToken.Value, firstToken.CodeLineIndex, firstToken.CodeLineNumber);
            }
            ExpressionNode root = BuildTree(tokens);
            if (OpenedBracketsLevel != 0)
            {
                throw new SyntaxErrorException("brackets do not match", tokens.Last().Value, tokens.Last().CodeLineIndex, tokens.Last().CodeLineNumber);
            }
            return root;
        }

        protected ExpressionNode BuildTree(IEnumerable<Token> tokens, ExpressionNode parent = null)
        {
            ExpressionNode root = null;
            ExpressionNode left = null;

            Token token = tokens.FirstOrDefault();
            if (token is null)
                return null;

            if (token.IsConstant || token.TokenType == Token.TokenTypes.ID || token.TokenType == Token.TokenTypes.BUILT_IN_FUNCTION)
            {
                left = new ExpressionNode()
                {
                    Operator = token,
                    Type = ExpressionNode.TokensToExpressionTypes.GetOrDefault(token.TokenType, ExpressionNode.ExpressionTypes.UNKNOWN)
                };
                var tt = tokens.ElementAtOrDefault(1)?.TokenType;
                if (tt == Token.TokenTypes.OPENING_ROUND_BRACKET)
                {
                    root = left;
                    left = null;
                    root.Type = ExpressionNode.ExpressionTypes.FUNCTION_CALL;
                    root.Right = BuildTree(tokens.Skip(1));

                }
                else if (tt == Token.TokenTypes.COLON)
                {
                    root = left;
                    left = null;
                    root.Right = BuildTree(tokens.Skip(2));
                    
                    // TODO: maybe throw error here if there is something after COLON
                }
                else
                {
                    root = BuildTree(tokens.Skip(1));
                    left.Parent = root;
                }
            }
            else if (token.IsOpeningBracket)
            {
                this.OpenedBracketsLevel++;
                root = BuildTree(tokens.Skip(1));

                root.OperatorPriority++;
            }
            else if (token.IsClosingBracket)
            {
                this.OpenedBracketsLevel--;
                root = BuildTree(tokens.Skip(1));
                if (root != null)
                    root.OperatorPriority--;
            }
            else if (token.IsOperation)
            {

                root = new ExpressionNode()
                {
                    Operator = token,
                    Type = ExpressionNode.TokensToExpressionTypes.GetOrDefault(token.TokenType, ExpressionNode.ExpressionTypes.UNKNOWN)
                };
                if (token.TokenType == Token.TokenTypes.MULTIPLICATION || token.TokenType == Token.TokenTypes.DIVISION)
                {
                    root.OperatorPriority++;
                }

                root.Right = BuildTree(tokens.Skip(1), root);
            }
            if (root is null)
            {
                if (left is null)
                    return null;
                left.Parent = parent;
                return left;
            }
            root.Parent = parent;
            if (left != null)
                root.InsertDeepLeft(left);

            if (root.Right != null && root.Operator.IsOperation && root.Right.Operator.IsOperation && root.OperatorPriority > root.Right.OperatorPriority)
                return root.LeftRotation();
            return root;
        }

        public static ExpressionNode ValidateNode(ExpressionNode node)
        {
            switch (node.Type)
            {
                case ExpressionNode.ExpressionTypes.BINARY_OPERATION:
                    
                        if (node.Left == null || node.Right == null)
                        {
                            throw new SyntaxErrorException(
                                "binary operation lacks operand",
                                node.Operator.Value,
                                node.Operator.CodeLineIndex,
                                node.Operator.CodeLineNumber
                                );
                        }
                        break;
                    
                case ExpressionNode.ExpressionTypes.BLOCK_OPENING_CONDITIONAL_OPERATION:
                    
                        if (node.Left != null || node.Right == null)
                            throw new SyntaxErrorException(
                                "conditional operator wrong usage",
                                node.Operator.Value,
                                node.Operator.CodeLineIndex,
                                node.Operator.CodeLineNumber
                                );
                        break;
                  
                case ExpressionNode.ExpressionTypes.UNKNOWN:
                    throw new SyntaxErrorException(
                        "unknown expression",
                        node.Operator.Value,
                        node.Operator.CodeLineIndex,
                        node.Operator.CodeLineNumber
                        );
                case ExpressionNode.ExpressionTypes.OPERAND:
                    if (node.Left != null)
                        throw new SyntaxErrorException(
                        "unknown operator",
                        node.Operator.Value,
                        node.Operator.CodeLineIndex,
                        node.Operator.CodeLineNumber
                        );
                    break;
                default:
                    break;
            }
            return node;
        }

        public class SyntaxErrorException : FormatException
        {
            public string Value { get; set; }
            public int PositionInLine { get; set; }
            public int LineNumber { get; set; }
            public SyntaxErrorException(string message, string value, int positionInLine, int lineNumber) : base(message)
            {
                Value = value;
                PositionInLine = positionInLine;
                LineNumber = lineNumber;
            }
        }

        public class ExpressionNode
        {
            public ExpressionNode Left = null;
            public Token Operator = null;
            public ExpressionTypes Type;
            public int OperatorPriority = 0;
            public ExpressionNode Right = null;
            public ExpressionNode Parent = null;
            public TreeList<ExpressionNode> Block = new TreeList<ExpressionNode>(null);

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
                UNARY_OPERATION,
                BINARY_OPERATION,
                BLOCK_OPENING_CONDITIONAL_OPERATION,
                BLOCK_OPENING_OPERATION,
                FUNCTION_CALL,
                FUNCTION_DEF,
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
                [TokenTypes.COLON] = ExpressionTypes.BLOCK_OPENING_OPERATION
            };
        }
    }
}
