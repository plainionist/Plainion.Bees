using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Plainion.AppFw.Shell.Forms;

namespace Plainion.Scripts.XamlEdit
{
    public class XamlEditor : FormsAppBase
    {
        public XamlEditor()
        {
            AssemblyPatterns = new List<IPattern>();
        }

        [Argument( Short = "-f", Description = "Xaml file to edit" ), Required]
        public string XamlFile
        {
            get;
            set;
        }

        public List<IPattern> AssemblyPatterns
        {
            get;
            private set;
        }

        protected override void Run()
        {
            var output = Path.Combine( Path.GetTempPath(), "XamlEdit" );

            var project = new VsCSharpProject();

            foreach ( var reference in GetReferences() )
            {
                project.AddReference( reference );
            }

            project.AddSource( XamlFile );

            var projecFile = Path.Combine( output, "XamlEdit.csproj" );

            project.WriteTo( projecFile );

            Process.Start( "devenv", projecFile ).WaitForExit();

            Directory.Delete( output, true );
        }

        private IEnumerable<string> GetReferences()
        {
            var assemblies = new List<string>();

            foreach ( var pattern in AssemblyPatterns )
            {
                var files = Directory.GetFiles( Path.GetDirectoryName( pattern.Value ), Path.GetFileName( pattern.Value ) );

                if ( pattern is Include )
                {
                    assemblies.AddRange( files );
                }
                else
                {
                    assemblies = assemblies.Except( files ).ToList();
                }
            }

            return assemblies;
        }
    }
}
