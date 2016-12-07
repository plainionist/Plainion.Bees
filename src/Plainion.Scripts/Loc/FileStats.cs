using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Plainion.Scripts.Loc
{
    public class FileStats
    {
        public static readonly string[] Source_Ext = { ".cs", ".cpp", ".h", ".xaml", ".aspx", ".fs" };

        public string Name
        {
            get;
            set;
        }
        public int SourceLines
        {
            get;
            set;
        }
        public int EmptyLines
        {
            get;
            set;
        }
        public int CommentLines
        {
            get;
            set;
        }
        public bool IsGenerated
        {
            get;
            set;
        }

        public static FileStats Create( string file )
        {
            FileStats stats = new FileStats();
            stats.Name = file;

            string filename = Path.GetFileName( file );
            stats.IsGenerated = filename.Contains( ".Designer." ) || filename == "Settings.cs";

            return stats;
        }

        public void Process()
        {
            string ext = Path.GetExtension( Name );
            if( !Source_Ext.Contains( ext ) )
            {
                return;
            }

            using( StreamReader reader = new StreamReader( Name ) )
            {
                bool inComment = false;
                while( !reader.EndOfStream )
                {
                    string line = reader.ReadLine().Trim();

                    if( line.Length == 0 )
                    {
                        ++EmptyLines;
                    }
                    else if( line.StartsWith( "/*" ) )
                    {
                        ++CommentLines;
                        if( !line.EndsWith( "*/" ) )
                        {
                            inComment = true;
                        }
                    }
                    else if( line.StartsWith( "//" ) )
                    {
                        ++CommentLines;
                    }
                    else if( inComment )
                    {
                        ++CommentLines;

                        if( line.EndsWith( "*/" ) )
                        {
                            inComment = false;
                        }
                    }
                    else
                    {
                        ++SourceLines;
                    }
                }
            }
        }

    }

}
