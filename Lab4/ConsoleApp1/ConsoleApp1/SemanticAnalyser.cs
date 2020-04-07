using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApp1.SemanticItem;


/*
  - трекать объявление переменных
  - объявление функций
  - скоупы

  - проверять объявленность переменных при их использовании
  - проверять типы операндов у бинарных операторов

  - проверять объявленность функций при их вызове
  - проверять число аргументов функции при вызове функции
     */
namespace ConsoleApp1
{
    class SemanticAnalyser
    {
        protected SemanticTreeList rootBlock;
        protected SemanticTreeList currentBlock;
        protected Dictionary<string, SemanticItem> definitions;

        public SemanticAnalyser(SemanticTreeList nodes)
        {
            rootBlock = nodes;
        }

        protected void AnalyseBlock(SemanticTreeList nodes)
        {
            if (nodes == null || nodes.Count == 0)
                return;

            foreach (var node in nodes)
            {
                AnalyseNodesTree(node);
                AnalyseBlock(node.Block);
            }
        }

        protected void AnalyseNodesTree(ExpressionNode node)
        {
            if (node == null)
                return;

            if (node.IsLeaf)
            {
                if (node.Operator.TokenType == Token.TokenTypes.ID && node.Parent.Operator.TokenType == Token.TokenTypes.ASSIGN)
                {
                    this.definitions.Add(node.Operator.Value, new SemanticItem()
                    {
                        Name = node.Operator.Value,
                        VarType = GetVarType(node.Operator),
                        
                    }
                    );
                }
            }
            //if (node.Left != null)
            //   AnalyseNodesTree(node.Left, localDefinitions);
            //if (node.Right != null)
            //   AnalyseNodesTree(node.Right, localDefinitions);
        }

        protected VarTypes GetVarType(Token token)
        {
            switch (token.TokenType)
            {
                case Token.TokenTypes.STRING_CONST:
                    return VarTypes.STRING_VAR;
                case Token.TokenTypes.INT_NUM:
                    return VarTypes.INTEGER_VAR;
                case Token.TokenTypes.FLOAT_NUM:
                    return VarTypes.FLOAT_VAR;
                default:
                    return VarTypes.NONE_VAR;
            }
        }
    }
}