using System;
using System.Xml.Linq;

namespace Plainion.Scripts.Loc
{
    public class CollectedStats
    {
        public CollectedStats( string dir )
        {
            Directory = dir;
        }

        public string Directory
        {
            get;
            private set;
        }

        public int ProductSourceLines
        {
            get;
            set;
        }
        public int ProductEmptyLines
        {
            get;
            set;
        }
        public int ProductCommentLines
        {
            get;
            set;
        }

        /// <summary>
        /// Generated product lines of code including empty lines and comments.
        /// </summary>
        public int GeneratedLines
        {
            get;
            set;
        }

        public int TestSourceLines
        {
            get;
            set;
        }
        public int TestEmptyLines
        {
            get;
            set;
        }
        public int TestCommentLines
        {
            get;
            set;
        }

        public void Sum( CollectedStats stats )
        {
            ProductSourceLines += stats.ProductSourceLines;
            ProductCommentLines += stats.ProductCommentLines;
            ProductEmptyLines += stats.ProductEmptyLines;

            GeneratedLines += stats.GeneratedLines;

            TestSourceLines += stats.TestSourceLines;
            TestCommentLines += stats.TestCommentLines;
            TestEmptyLines += stats.TestEmptyLines;
        }

        public XElement ToXml()
        {
            return new XElement( "loc-report", ToXml( this ) );
        }

        public XElement ToXml( CollectedStats collected )
        {
            return new XElement( "directory",
                new XAttribute( "name", Directory ),
                new XAttribute( "total", collected.ProductSourceLines + collected.ProductCommentLines +
                    collected.ProductEmptyLines + collected.GeneratedLines +
                    collected.TestSourceLines + collected.TestCommentLines + collected.TestEmptyLines ),
                new XAttribute( "PSLOC", collected.ProductSourceLines ),
                new XAttribute( "PCLOC", collected.ProductCommentLines ),
                new XAttribute( "PELOC", collected.ProductEmptyLines ),
                new XAttribute( "GLOC", collected.GeneratedLines ),
                new XAttribute( "TSLOC", collected.TestSourceLines ),
                new XAttribute( "TCLOC", collected.TestCommentLines ),
                new XAttribute( "TELOC", collected.TestEmptyLines ) );
        }

        public void Print()
        {
            Console.WriteLine( "{0,30} {1,7} {2,7} {3,7} {4,7} {5,7} {6,7} {7,7} {8,7}",
                "Directory", "Total", "PSLOC", "PCLOC", "PELOC", "GLOC", "TSLOC", "TCLOC", "TELOC" );
            Print( this );
        }

        private void Print( CollectedStats collected )
        {
            Console.WriteLine( "{0,30} {1,7} {2,7} {3,7} {4,7} {5,7} {6,7} {7,7} {8,7}",
                Directory,
                collected.ProductSourceLines + collected.ProductCommentLines +
                collected.ProductEmptyLines + collected.GeneratedLines +
                collected.TestSourceLines + collected.TestCommentLines + collected.TestEmptyLines,

                collected.ProductSourceLines,
                collected.ProductCommentLines,
                collected.ProductEmptyLines,
                collected.GeneratedLines,
                collected.TestSourceLines,
                collected.TestCommentLines,
                collected.TestEmptyLines );
        }
    }

    public static class TreeNodeExtension
    {
        public static CollectedStats CollectedStats( this TreeNode node )
        {
            return (CollectedStats) node.Tag;
        }
    }
}
