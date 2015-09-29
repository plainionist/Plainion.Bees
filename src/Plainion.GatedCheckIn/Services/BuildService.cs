using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Xml;
using Plainion.GatedCheckIn.Model;

namespace Plainion.GatedCheckIn.Services
{
    [Export]
    class BuildService : IDisposable
    {
        private GitService myGitService;
        private string myBuildDefinitionLocation;

        [ImportingConstructor]
        public BuildService(GitService gitService)
        {
            myGitService = gitService;
        }

        public void Dispose()
        {
            if (myBuildDefinitionLocation != null)
            {
                SaveBuildDefinition();
                myBuildDefinitionLocation = null;
            }
        }

        public BuildDefinition BuildDefinition { get; private set; }

        public event Action BuildDefinitionChanged;

        public void InitializeBuildDefinition(string path)
        {
            Contract.RequiresNotNull(path, "path");

            myBuildDefinitionLocation = path;

            if (!File.Exists(myBuildDefinitionLocation))
            {
                BuildDefinition = CreateDefaultBuildDefinition();
                BuildDefinition.RepositoryRoot = Path.GetFullPath(Path.GetDirectoryName(myBuildDefinitionLocation));

                SaveBuildDefinition();
            }
            else
            {
                using (var reader = XmlReader.Create(myBuildDefinitionLocation))
                {
                    var serializer = new DataContractSerializer(typeof(BuildDefinition));
                    BuildDefinition = (BuildDefinition)serializer.ReadObject(reader);
                }
            }

            if (BuildDefinitionChanged != null)
            {
                BuildDefinitionChanged();
            }
        }

        private void SaveBuildDefinition()
        {
            using (var writer = XmlWriter.Create(myBuildDefinitionLocation))
            {
                var serializer = new DataContractSerializer(typeof(BuildDefinition));
                serializer.WriteObject(writer, BuildDefinition);
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
                TestRunnerExecutable = @"\Extern\NUnit\bin\nunit-console.exe"
            };
        }

        public Task<bool> ExecuteAsync(BuildRequest request, IProgress<string> progress)
        {
            Contract.Invariant(BuildDefinition != null, "BuildDefinition not loaded");

            return new BuildWorkflow(myGitService, BuildDefinition, request)
                .ExecuteAsync(progress);
        }
    }
}
