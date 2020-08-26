using System.Collections;
using System.Collections.Generic;

namespace Plainion.Scripts.Loc
{
    public class TreeNode
    {
        public TreeNode(string name)
        {
            Name = name;
            Nodes = new List<TreeNode>();
        }

        public string Name { get; }
        public object Tag { get; set; }
        public IList<TreeNode> Nodes { get; }
    }
}