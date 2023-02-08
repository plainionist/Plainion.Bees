using System;
using System.Collections.Generic;
using System.IO;

namespace Plainion.Scripts.Loc
{
    public class DirStats
    {
        public static List<string> DirectoryExcludes { get; } = new List<string> { "CVS", "obj", "bin", "Properties", "lost+found", ".hg" };

        private bool myAreTests = false;

        public DirStats(DirStats parent)
        {
            Parent = parent;
            Directories = new List<DirStats>();
            Files = new List<FileStats>();
        }

        public DirStats Parent
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            set;
        }

        public List<DirStats> Directories
        {
            get;
            set;
        }

        public List<FileStats> Files
        {
            get;
            set;
        }

        public bool AreTests
        {
            get
            {
                DirStats ds = this;
                while (ds != null)
                {
                    if (ds.myAreTests)
                    {
                        return true;
                    }

                    ds = ds.Parent;
                }

                return false;
            }
            set
            {
                myAreTests = value;
            }
        }

        public static DirStats Create(DirStats parent, string dir)
        {
            DirStats stats = new DirStats(parent);
            stats.Name = dir;
            string filename = Path.GetFileName(dir);
            stats.AreTests = filename == "test.u" || filename == "test.i" ||
                filename.EndsWith("_uTest") || filename.EndsWith("_iTest") ||
                filename.EndsWith("Tests");

            return stats;
        }

        public void Process()
        {
            string[] files = Directory.GetFileSystemEntries(Name);
            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                if (DirectoryExcludes.Contains(name))
                {
                    continue;
                }

                if (DirectoryExcludes.Contains(file))
                {
                    continue;
                }

                Console.WriteLine("Processing: " + Name);

                if (Directory.Exists(file))
                {
                    DirStats stats = DirStats.Create(this, file);
                    Directories.Add(stats);

                    stats.Process();
                }
                else
                {
                    FileStats stats = FileStats.Create(file);
                    Files.Add(stats);

                    stats.Process();
                }
            }
        }

        public TreeNode BuildTree()
        {
            TreeNode root = new TreeNode(Name);
            root.Tag = this;

            BuildTree(this, root);
            root = root.Nodes[0];

            return root;
        }

        private CollectedStats BuildTree(DirStats stats, TreeNode root)
        {
            TreeNode node = new TreeNode(Path.GetFileName(stats.Name));
            root.Nodes.Add(node);

            CollectedStats collected = new CollectedStats(stats.Name);
            foreach (DirStats child in stats.Directories)
            {
                collected.Sum(BuildTree(child, node));
            }

            foreach (FileStats child in stats.Files)
            {
                if (child.IsGenerated && !stats.AreTests)
                {
                    collected.GeneratedLines += child.CommentLines;
                    collected.GeneratedLines += child.EmptyLines;
                    collected.GeneratedLines += child.SourceLines;
                }
                else if (stats.AreTests)
                {
                    collected.TestCommentLines += child.CommentLines;
                    collected.TestEmptyLines += child.EmptyLines;
                    collected.TestSourceLines += child.SourceLines;
                }
                else
                {
                    collected.ProductCommentLines += child.CommentLines;
                    collected.ProductEmptyLines += child.EmptyLines;
                    collected.ProductSourceLines += child.SourceLines;
                }
            }

            node.Tag = collected;

            return collected;
        }

    }
}
