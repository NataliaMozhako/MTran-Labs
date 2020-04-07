using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class TreeList<T> : List<T> where T : class
    {
        public TreeList<T> Parent = null;
        public int Indentation = 0;
        public TreeList(TreeList<T> parent)
        {
            Parent = parent;
        }
    }
}
