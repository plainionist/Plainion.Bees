using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml;
using Plainion.AppFw.Shell.Forms;

namespace Plainion.Scripts.Loc
{
    public class LinesOfCode : FormsAppBase
    {
        [Argument( Short = "-s", Description = "Project sources root" )]
        public string Source
        {
            get;
            set;
        }

        [Argument( Short = "-o", Long = "-output", Description = "Report output file" )]
        public string Output
        {
            get;
            set;
        }

        protected override void Run()
        {
            if ( !string.IsNullOrEmpty( Source ) )
            {
                RunBatch();
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault( false );
                Application.Run( new MainForm() );
            }
        }

        private void RunBatch()
        {
            var stats = DirStats.Create( null, Source );

            stats.Process();

            var tree = stats.BuildTree();
            tree.CollectedStats().Print();

            if ( Output != null )
            {
                var settings = new XmlWriterSettings();
                settings.Indent = true;

                var writer = XmlWriter.Create( Output, settings );

                tree.CollectedStats().ToXml().WriteTo( writer );
                writer.Close();
            }
        }
    }
}
