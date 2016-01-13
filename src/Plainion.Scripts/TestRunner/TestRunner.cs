using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Plainion.AppFw.Shell.Forms;
using Plainion.IO;
using Plainion.Logging;

namespace Plainion.Scripts.TestRunner
{
    public class TestRunner : FormsAppBase
    {
        private static readonly ILogger myLogger = LoggerFactory.GetLogger(typeof(TestRunner));

        public TestRunner()
        {
            WorkingDirectory = ".";
        }

        [Argument(Short = "-g", Long = "-gui", Description = "Use the GUI runner")]
        public bool WithGui { get; set; }

        [Required]
        [Argument(Short = "-a", Long = "-assembly", Description = "Test assemblies to execute")]
        public string Assemblies { get; set; }

        public string NUnitGui { get; set; }

        public string NUnitConsole { get; set; }

        public string WorkingDirectory { get; set; }

        public bool Succeeded { get; private set; }

        protected override void Run()
        {
            var nunitProject = GenerateProject();
            if (nunitProject == null)
            {
                return;
            }

            if (WithGui)
            {
                Contract.Requires(File.Exists(NUnitGui), "Runner executable not found: {0}", NUnitGui);

                var process = Process.Start(NUnitGui, string.Format("{0} /run", nunitProject));
                process.WaitForExit();
                Succeeded = process.ExitCode == 0;
            }
            else
            {
                Contract.Requires(File.Exists(NUnitConsole), "Runner executable not found: {0}", NUnitConsole);

                var info = new ProcessStartInfo(NUnitConsole, nunitProject);
                info.UseShellExecute = false;

                var process = Process.Start(info);
                process.WaitForExit();

                Succeeded = process.ExitCode == 0;
            }

            //File.Delete( nunitProject );
        }

        public string GenerateProject()
        {
            var testAssemblies = ResolveTestAssemblies();

            if (!testAssemblies.Any())
            {
                myLogger.Error("No test assemblies found");
                return null;
            }

            // assume shortest folder is the root folder
            var testFolder = testAssemblies
                .Select(path => Path.GetDirectoryName(path))
                .OrderBy(dir => dir.Length)
                .First();

            var project = new XElement("NUnitProject",
                new XElement("Settings",
                    new XAttribute("activeconfig", "default"),
                    new XAttribute("appbase", testFolder)),
                new XElement("Config", new XAttribute("name", "default"),
                    testAssemblies
                        .Select(assembly => new XElement("assembly", new XAttribute("path", assembly)))
                    ));

            var projectFile = Path.Combine(testFolder, "Plainion.gen.nunit");
            using (var writer = XmlWriter.Create(projectFile))
            {
                project.WriteTo(writer);
            }

            myLogger.Info("NUnit project written to: {0}", projectFile);

            return projectFile;
        }

        private IEnumerable<string> ResolveTestAssemblies()
        {
            var assemblyPatterns = Assemblies.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

            return assemblyPatterns
                .SelectMany(pattern => ResolveFilePattern(pattern))
                .Distinct()
                .ToList();
        }

        private IEnumerable<string> ResolveFilePattern(string pattern)
        {
            var directory = Path.GetDirectoryName(pattern);
            if (string.IsNullOrWhiteSpace(directory))
            {
                directory = Path.GetFullPath(WorkingDirectory);
            }

            var filePattern = Path.GetFileName(pattern);

            myLogger.Notice("Searching in {0} for {1}", directory, filePattern);

            var testAssemblies = Directory.GetFiles(directory, filePattern)
                .Where(file => Path.GetExtension(file).Equals(".dll", StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var file in testAssemblies)
            {
                myLogger.Notice("  -> {0}", file);
            }

            return testAssemblies;
        }
    }
}
