using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Plainion.GatedCheckIn.Model;
using Plainion.IO;
using Plainion.Scripts.TestRunner;

namespace Plainion.GatedCheckIn.Services
{
    internal class BuildWorkflow
    {
        private GitService myGitService;
        private BuildDefinition myDefinition;
        private BuildRequest myRequest;

        public BuildWorkflow(GitService gitService, BuildDefinition definition, BuildRequest request)
        {
            myGitService = gitService;
            myDefinition = definition;
            myRequest = request;
        }

        internal Task<bool> ExecuteAsync(IProgress<string> progress)
        {
            // clone thread save copy of the relevant paramters;
            myDefinition = Objects.Clone(myDefinition);
            myRequest = Objects.Clone(myRequest);

            return Task<bool>.Run(() => BuildSolution( progress)
                                        && RunTests( progress)
                                        && CheckIn( progress));
        }

        private bool BuildSolution( IProgress<string> progress)
        {
            return ExecuteWithOutputRedirection(writer =>
            {
                var info = new ProcessStartInfo(@"C:\Program Files (x86)\MSBuild\12.0\Bin\MSBuild.exe",
                   "/m /p:Configuration=" + myDefinition.Configuration +
                   " /p:Platform=\"" + myDefinition.Platform + "\" " +
                   " /p:OutputPath=\"" + GetWorkingDirectory() + "\"" +
                   " " + Path.Combine(myDefinition.RepositoryRoot, myDefinition.Solution));

                info.CreateNoWindow = true;

                var ret = Processes.Execute(info, writer, writer);
                return ret == 0;
            }, progress);
        }

        private string GetWorkingDirectory()
        {
            return Path.Combine(myDefinition.RepositoryRoot, "bin", "gc");
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

        private bool RunTests( IProgress<string> progress)
        {
            if (!myDefinition.RunTests)
            {
                return true;
            }

            return ExecuteWithOutputRedirection(writer =>
            {
                var runner = new TestRunner
                {
                    NUnitConsole = myDefinition.TestRunnerExecutable,
                    WithGui = false,
                    Assemblies = myDefinition.TestAssemblyPattern,
                };

                var stdOut = Console.Out;
                Console.SetOut(writer);
                runner.ExecuteEmbedded(GetWorkingDirectory(), writer, writer);
                Console.SetOut(stdOut);

                return runner.Succeeded;
            }, progress);
        }

        private bool CheckIn( IProgress<string> progress)
        {
            if (!myDefinition.CheckIn)
            {
                return true;
            }

            if (string.IsNullOrEmpty(myRequest.CheckInComment))
            {
                progress.Report("!! NO CHECKIN COMMENT PROVIDED !!");
                return false;
            }

            try
            {
                myGitService.Commit(myDefinition.RepositoryRoot, myRequest.Files, myRequest.CheckInComment, myDefinition.UserName, myDefinition.UserEMail);
                
                progress.Report("--- CHECKIN SUCCEEDED ---");

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
