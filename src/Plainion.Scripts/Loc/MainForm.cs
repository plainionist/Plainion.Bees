using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Plainion.Scripts.Loc
{
    public partial class MainForm : Form
    {
        private DirStats myStats = null;

        public MainForm()
        {
            InitializeComponent();
        }

        private void myRunBtn_Click( object sender, EventArgs e )
        {
            if( string.IsNullOrEmpty( myDir.Text ) )
            {
                return;
            }

            myTree.Nodes.Clear();

            myRunBtn.Enabled = false;

            myStats = DirStats.Create( null, myDir.Text );
            myStats.Process();

            PrintResult( myStats );

            myRunBtn.Enabled = true;
        }

        private void PrintResult( DirStats stats )
        {
            TreeNode root = stats.BuildTree();

            myTree.Nodes.Add( root );
            root.Expand();
            myTree.SelectedNode = root;
            myTree.Select();

            myTable.Rows.Clear();

            PrintCollectedStats( root, 0 );
        }

        private int PrintCollectedStats( TreeNode node, int pos )
        {
            if( !node.IsExpanded && !node.IsVisible )
            {
                return -1;
            }

            while( myTable.Rows.Count - 1 < pos )
            {
                DataGridViewRow row = new DataGridViewRow();
                row.Height = myTree.Font.Height + 3;
                myTable.Rows.Add( row );
            }

            CollectedStats collected = (CollectedStats) node.Tag;

            StringBuilder sb = new StringBuilder();

            myTable[ "Total", pos ].Value = PrintValue(
                collected.ProductSourceLines + collected.ProductCommentLines +
                collected.ProductEmptyLines + collected.GeneratedLines +
                collected.TestSourceLines + collected.TestCommentLines + collected.TestEmptyLines );

            myTable[ "PSLOC", pos ].Value = PrintValue( collected.ProductSourceLines );
            myTable[ "PCLOC", pos ].Value = PrintValue( collected.ProductCommentLines );
            myTable[ "PELOC", pos ].Value = PrintValue( collected.ProductEmptyLines );

            myTable[ "GLOC", pos ].Value = PrintValue( collected.GeneratedLines );

            myTable[ "TSLOC", pos ].Value = PrintValue( collected.TestSourceLines );
            myTable[ "TCLOC", pos ].Value = PrintValue( collected.TestCommentLines );
            myTable[ "TELOC", pos ].Value = PrintValue( collected.TestEmptyLines );

            ++pos;

            foreach( TreeNode child in node.Nodes )
            {
                int p = PrintCollectedStats( child, pos );
                if( p != -1 )
                {
                    pos = p;
                }
            }

            return pos;
        }

        private string PrintValue( int value )
        {
            if( value == 0 )
            {
                return "-";
            }

            return string.Format( "{0:0,0}", value );
        }

        private void MainForm_Load( object sender, EventArgs e )
        {
            myTable.Font = myTree.Font;
            myTable.Rows.Clear();
        }

        private void myTree_AfterExpand( object sender, TreeViewEventArgs e )
        {
            myTable.Rows.Clear();
            PrintCollectedStats( myTree.Nodes[ 0 ], 0 );

            SelectNode( myTree.SelectedNode );
        }

        private void myTree_AfterCollapse( object sender, TreeViewEventArgs e )
        {
            myTable.Rows.Clear();
            PrintCollectedStats( myTree.Nodes[ 0 ], 0 );

            SelectNode( myTree.SelectedNode );
        }

        private void myTree_AfterSelect( object sender, TreeViewEventArgs e )
        {
            SelectNode( e.Node );
        }

        private void SelectNode( TreeNode node )
        {
            // if the given node is not visible take its parent
            TreeNode n = node;
            while( n != null && !n.IsVisible )
            {
                n = n.Parent;
            }

            if( n == null )
            {
                return;
            }

            int pos = 0;
            GetNodeRow( myTree.Nodes[ 0 ], ref pos, n );

            myTable.ClearSelection();
            myTable.Rows[ pos ].Selected = true;
        }

        private bool GetNodeRow( TreeNode root, ref int pos, TreeNode node )
        {
            if( root == node )
            {
                return true;
            }

            foreach( TreeNode n in root.Nodes )
            {
                ++pos;

                if( n == node )
                {
                    return true;
                }

                if( n.IsExpanded )
                {
                    if( GetNodeRow( n, ref pos, node ) )
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void myDumpBtn_Click( object sender, EventArgs e )
        {
            string file = Path.Combine( Environment.CurrentDirectory, CreateTimestamp() + ".csv" );
            using( StreamWriter writer = new StreamWriter( file ) )
            {
                writer.WriteLine( "Directory;Total;PSLOC;PCLOC;PELOC;GLOC;TSLOC;TCLOC;TELOC" );

                Dump( myTree.Nodes[ 0 ], writer );
            }

            MessageBox.Show( "Snapshot dumped to: " + file );
        }

        private string CreateTimestamp()
        {
            return string.Format( "{0:yyyy-MM-dd_HH-mm}", DateTime.Now );
        }

        private void Dump( TreeNode node, StreamWriter writer )
        {
            CollectedStats collected = (CollectedStats) node.Tag;

            writer.Write( collected.Directory );
            writer.Write( ";" );

            writer.Write( collected.ProductSourceLines + collected.ProductCommentLines +
                collected.ProductEmptyLines + collected.GeneratedLines +
                collected.TestSourceLines + collected.TestCommentLines + collected.TestEmptyLines );
            writer.Write( ";" );

            writer.Write( collected.ProductSourceLines );
            writer.Write( ";" );
            writer.Write( collected.ProductCommentLines );
            writer.Write( ";" );
            writer.Write( collected.ProductEmptyLines );
            writer.Write( ";" );

            writer.Write( collected.GeneratedLines );
            writer.Write( ";" );

            writer.Write( collected.TestSourceLines );
            writer.Write( ";" );
            writer.Write( collected.TestCommentLines );
            writer.Write( ";" );
            writer.Write( collected.TestEmptyLines );
            writer.WriteLine();

            foreach( TreeNode child in node.Nodes )
            {
                Dump( child, writer );
            }
        }
    }
}
