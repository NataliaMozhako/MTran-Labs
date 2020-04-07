using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApp1.LexicalAnalyzer;
using static ConsoleApp1.Token;

namespace ConsoleApp1
{
    class SyntaxAnalyzer
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
                if (tt == TokenTypes.OPENING_ROUND_BRACKET || tt == TokenTypes.OPENING_SQUARE_BRACKET)
                {
                    root = left;
                    left = null;
                    if (tt == TokenTypes.OPENING_SQUARE_BRACKET)
                    {
                        root.Type = ExpressionNode.ExpressionTypes.INDEXER_CALL;
                    }
                    else
                    {
                        root.Type = ExpressionNode.ExpressionTypes.FUNCTION_CALL;
                    }
                    root.Right = BuildTree(tokens.Skip(1));

                }
                
                //
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
                if (root != null)
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

        
    }
}
