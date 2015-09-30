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

        [ImportingConstructor]
        public BuildService(GitService gitService)
        {
            myGitService = gitService;
        }

        public void Dispose()
        {
            SaveBuildDefinitionOnDemand();
        }

        public BuildDefinition BuildDefinition { get; private set; }

        public event Action BuildDefinitionChanged;

        public void InitializeBuildDefinition(string repositoryRoot)
        {
            if (string.IsNullOrEmpty(repositoryRoot) || !File.Exists(repositoryRoot))
            {
                BuildDefinition = CreateDefaultBuildDefinition();
            }
            else
            {
                using (var reader = XmlReader.Create(repositoryRoot))
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

        private void SaveBuildDefinitionOnDemand()
        {
            if (BuildDefinition == null || BuildDefinition.RepositoryRoot == null)
            {
                return;
            }

            var file = Path.Combine(BuildDefinition.RepositoryRoot, Path.GetFileName(BuildDefinition.RepositoryRoot) + ".gc");
            using (var writer = XmlWriter.Create(file))
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
