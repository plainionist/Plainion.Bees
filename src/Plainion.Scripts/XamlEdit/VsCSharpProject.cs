using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Plainion.Scripts.XamlEdit
{
    public class VsCSharpProject
    {
        private XElement myRoot;

        public VsCSharpProject()
        {
            myRoot = CreateRoot();
            myRoot.Add( CreateGlobalProperties() );
            myRoot.Add( CreateDefaultReferences() );
            myRoot.Add( CreateDefaultSources() );
            myRoot.Add( CreateTargetsImport() );
        }

        private static XElement XElement( string name, params object[] content )
        {
            return new XElement( XName.Get( name, "http://schemas.microsoft.com/developer/msbuild/2003" ), content );
        }

        private XElement CreateRoot()
        {
            return XElement( "Project",
                new XAttribute( "ToolsVersion", "4.0" ),
                new XAttribute( "DefaultTargets", "Build" ) );
        }

        private XElement CreateGlobalProperties()
        {
            return XElement( "PropertyGroup",
                XElement( "ProjectGuid", "{4F579366-1776-4AD7-AE3B-031D3242099F}" ),
                XElement( "OutputType", "Library" ),
                XElement( "AssemblyName", "XamlEditor" ),
                XElement( "TargetFrameworkVersion", "v4.0" ),
                XElement( "OutputPath", "." ),
                XElement( "ErrorReport", "prompt" ) );
        }

        private XElement CreateDefaultReferences()
        {
            return XElement( "ItemGroup",
                XElement( "Reference", new XAttribute( "Include", "PresentationCore" ) ),
                XElement( "Reference", new XAttribute( "Include", "PresentationFramework" ) ),
                XElement( "Reference", new XAttribute( "Include", "System" ) ),
                XElement( "Reference", new XAttribute( "Include", "System.Core" ) ),
                XElement( "Reference", new XAttribute( "Include", "System.Xaml" ) ),
                XElement( "Reference", new XAttribute( "Include", "WindowsBase" ) ) );
        }

        private XElement CreateDefaultSources()
        {
            return XElement( "ItemGroup",
                XElement( "Compile",
                    new XAttribute( "Include", "AssemblyInfo.cs" ) ) );
        }

        private XElement CreateTargetsImport()
        {
            return XElement( "Import",
                new XAttribute( "Project", @"$(MSBuildToolsPath)\Microsoft.CSharp.targets" ) );
        }

        public void AddSource( string file )
        {
            myRoot.Add( XElement( "ItemGroup",
                XElement( "Content",
                    new XAttribute( "Include", file ) ) ) );
        }

        internal void AddReference( string reference )
        {
            myRoot.Add( XElement( "ItemGroup",
                XElement( "Reference",
                    new XAttribute( "Include", Path.GetFileNameWithoutExtension( reference ) ),
                    XElement( "HintPath", Path.GetFullPath( reference ) ) ) ) );
        }

        public void WriteTo( string projecFile )
        {
            var dir = Path.GetDirectoryName( projecFile );
            if ( !Directory.Exists( dir ) )
            {
                Directory.CreateDirectory( dir );
            }

            WriteProjectFile( projecFile );

            var assemblyInfo = Path.Combine( dir, "AssemblyInfo.cs" );
            WriteAssemblyInfo( assemblyInfo );
        }

        private void WriteProjectFile( string projecFile )
        {
            var settings = new XmlWriterSettings();
            settings.Indent = true;

            using ( var xmlWriter = XmlWriter.Create( projecFile ) )
            {
                myRoot.WriteTo( xmlWriter );
            }
        }

        private void WriteAssemblyInfo( string assemblyInfo )
        {
            using ( var writer = new StreamWriter( assemblyInfo ) )
            {
                writer.WriteLine( "using System.Reflection;" );
                writer.WriteLine( "[assembly: AssemblyVersion( \"1.0.0.0\" )]" );
                writer.WriteLine( "[assembly: AssemblyFileVersion( \"1.0.0.0\" )]" );
            }
        }
    }
}
