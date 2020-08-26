using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

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

        public XElement ToXml()
        {
            return new XElement("loc-report", ToXml(this));
        }

        private static XElement ToXml(TreeNode node)
        {
            var collected = (CollectedStats)node.Tag;
            return new XElement("directory",
                new XAttribute("name", node.Name),
                new XAttribute("total", collected.ProductSourceLines + collected.ProductCommentLines +
                    collected.ProductEmptyLines + collected.GeneratedLines +
                    collected.TestSourceLines + collected.TestCommentLines + collected.TestEmptyLines),
                new XAttribute("PSLOC", collected.ProductSourceLines),
                new XAttribute("PCLOC", collected.ProductCommentLines),
                new XAttribute("PELOC", collected.ProductEmptyLines),
                new XAttribute("GLOC", collected.GeneratedLines),
                new XAttribute("TSLOC", collected.TestSourceLines),
                new XAttribute("TCLOC", collected.TestCommentLines),
                new XAttribute("TELOC", collected.TestEmptyLines),
                node.Nodes.Select(ToXml));
        }
    }
}