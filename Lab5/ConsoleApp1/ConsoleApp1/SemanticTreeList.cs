using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class SemanticTreeList : List<ExpressionNode>
    {
        public Dictionary<string, SemanticItem> LocalDefinitions = new Dictionary<string, SemanticItem>();
        public SemanticTreeList Parent = null;
        public int Indentation = 0;

        public SemanticTreeList(SemanticTreeList parent)
        {
            this.Parent = parent;
        }
    }
}
