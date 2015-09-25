﻿using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Plainion.GatedCheckIn.Model;
using Plainion.IO;
using Plainion.Scripts.TestRunner;

namespace Plainion.GatedCheckIn.Services
{
    [Export]
    class BuildService
    {
        private GitService myGitService;

        [ImportingConstructor]
        public BuildService(GitService gitService)
        {
            myGitService = gitService;
        }

        public BuildDefinition BuildDefinition { get; private set; }

        public event Action BuildDefinitionChanged;

        public void InitializeBuildDefinition(string path)
        {
            Contract.RequiresNotNull(path, "path");

            if (!File.Exists(path))
            {
                BuildDefinition = CreateDefaultBuildDefinition();
                BuildDefinition.RepositoryRoot = Path.GetFullPath(Path.GetDirectoryName(path));

                using (var writer = XmlWriter.Create(path))
                {
                    var serializer = new DataContractSerializer(typeof(BuildDefinition));
                    serializer.WriteObject(writer, BuildDefinition);
                }
            }
            else
            {
                using (var reader = XmlReader.Create(path))
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

            return Task<bool>.Run(() => BuildSolution(request, progress)
                                        && RunTests(request, progress)
                                        && CheckIn(request, progress));
        }

        private bool BuildSolution(BuildRequest request, IProgress<string> progress)
        {
            return ExecuteWithOutputRedirection(writer =>
            {
                var info = new ProcessStartInfo(@"C:\Program Files (x86)\MSBuild\12.0\Bin\MSBuild.exe",
                   "/m /p:Configuration=" + BuildDefinition.Configuration +
                   " /p:Platform=\"" + BuildDefinition.Platform + "\" " +
                   " /p:OutputPath=\"" + GetWorkingDirectory() + "\"" +
                   " " + Path.Combine(BuildDefinition.RepositoryRoot, BuildDefinition.Solution));

                info.CreateNoWindow = true;

                var ret = Processes.Execute(info, writer, writer);
                return ret == 0;
            }, progress);
        }

        private string GetWorkingDirectory()
        {
            return Path.Combine(BuildDefinition.RepositoryRoot, "bin", "gc");
        }

        private bool ExecuteWithOutputRedirection(Func<TextWriter, bool> Executor, IProgress<string> progress)
        {
            using (var stream = new ProducerConsumerStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        var finishedToken = Guid.NewGuid().ToString();

                        var cts = new CancellationTokenSource();
                        var task = Task.Run(() =>
                        {
                            while (!cts.Token.IsCancellationRequested)
                            {
                                var line = reader.ReadLine();
                                if (line == null)
                                {
                                    continue;
                                }
                                if (line == finishedToken)
                                {
                                    break;
                                }
                                progress.Report(line);
                            }
                        }, cts.Token);

                        try
                        {
                            var ret = Executor(writer);

                            writer.WriteLine();
                            writer.WriteLine(finishedToken);

                            writer.Flush();
                            stream.Flush();

                            task.Wait();

                            return ret;
                        }
                        catch (Exception ex)
                        {
                            cts.Cancel();

                            throw new Exception("Failed to build solution", ex);
                        }
                    }
                }
            }
        }

        private bool RunTests(BuildRequest request, IProgress<string> progress)
        {
            if (!BuildDefinition.RunTests)
            {
                return true;
            }

            return ExecuteWithOutputRedirection(writer =>
                {
                    var runner = new TestRunner
                    {
                        NUnitConsole = BuildDefinition.TestRunnerExecutable,
                        WithGui = false,
                        Assemblies = BuildDefinition.TestAssemblyPattern,
                    };

                    var stdOut = Console.Out;
                    Console.SetOut(writer);
                    runner.ExecuteEmbedded(GetWorkingDirectory(), writer, writer);
                    Console.SetOut(stdOut);

                    return runner.Succeeded;
                }, progress);
        }

        private bool CheckIn(BuildRequest request, IProgress<string> progress)
        {
            if (!BuildDefinition.CheckIn)
            {
                return true;
            }

            if (string.IsNullOrEmpty(request.CheckInComment))
            {
                progress.Report("!! NO CHECKIN COMMENT PROVIDED !!");
                return false;
            }

            try
            {
                myGitService.Commit(BuildDefinition.RepositoryRoot, request.Files, request.CheckInComment, request.UserName, request.UserEMail);
                return true;
            }
            catch (Exception ex)
            {
                progress.Report("CHECKIN FAILED: " + ex.Message);
                return false;
            }
        }
    }
}