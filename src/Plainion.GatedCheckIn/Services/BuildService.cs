using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml;
using Plainion.GatedCheckIn.Model;
using Plainion.GatedCheckIn.Services.SourceControl;

namespace Plainion.GatedCheckIn.Services
{
    [Export]
    class BuildService : IDisposable
    {
        private ISourceControl mySourceControl;

        [ImportingConstructor]
        public BuildService( ISourceControl sourceControl )
        {
            mySourceControl = sourceControl;
        }

        public void Dispose()
        {
            SaveBuildDefinitionOnDemand();
        }

        public BuildDefinition BuildDefinition { get; private set; }

        public event Action BuildDefinitionChanged;

        public void InitializeBuildDefinition( string buildDefinitionFile )
        {
            if( string.IsNullOrEmpty( buildDefinitionFile ) || !File.Exists( buildDefinitionFile ) )
            {
                BuildDefinition = CreateDefaultBuildDefinition();
            }
            else
            {
                using( var reader = XmlReader.Create( buildDefinitionFile ) )
                {
                    var serializer = new DataContractSerializer( typeof( BuildDefinition ) );
                    BuildDefinition = ( BuildDefinition )serializer.ReadObject( reader );
                }
            }

            BuildDefinition.RepositoryRoot = Path.GetDirectoryName( buildDefinitionFile );

            if( BuildDefinitionChanged != null )
            {
                BuildDefinitionChanged();
            }
        }

        private void SaveBuildDefinitionOnDemand()
        {
            if( BuildDefinition == null || BuildDefinition.RepositoryRoot == null )
            {
                return;
            }

            var file = Path.Combine( BuildDefinition.RepositoryRoot, Path.GetFileName( BuildDefinition.RepositoryRoot ) + ".gc" );
            using( var writer = XmlWriter.Create( file ) )
            {
                var serializer = new DataContractSerializer( typeof( BuildDefinition ) );
                serializer.WriteObject( writer, BuildDefinition );
            }
        }

        private BuildDefinition CreateDefaultBuildDefinition()
        {
            return new BuildDefinition
            {
                CheckIn = true,
                Configuration = "Debug",
                Platform = "Any CPU",
                RunTests = true,
                TestAssemblyPattern = "*Tests.dll",
                TestRunnerExecutable = @"\bin\NUnit\bin\nunit-console.exe"
            };
        }

        public Task<bool> ExecuteAsync( BuildRequest request, IProgress<string> progress )
        {
            Contract.Invariant( BuildDefinition != null, "BuildDefinition not loaded" );

            return new BuildWorkflow( mySourceControl, BuildDefinition, request )
                .ExecuteAsync( progress );
        }
    }
}
