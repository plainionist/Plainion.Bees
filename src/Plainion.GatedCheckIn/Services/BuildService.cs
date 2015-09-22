using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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

        public Task<bool> ExecuteAsync(BuildRequest settings, IProgress<string> progress)
        {
            return Task<bool>.Run(() => BuildSolution(settings, progress)
                                        && RunTests(settings, progress)
                                        && CheckIn(settings, progress));
        }

        private bool BuildSolution(BuildRequest settings, IProgress<string> progress)
        {
            return ExecuteWithOutputRedirection(writer =>
            {
                var info = new ProcessStartInfo(@"C:\Program Files (x86)\MSBuild\12.0\Bin\MSBuild.exe",
                   "/m /p:Configuration=" + settings.Configuration +
                   " /p:Platform=\"" + settings.Platform + "\" " +
                   " /p:OutputPath=\"" + GetWorkingDirectory(settings) + "\"" +
                   " " + settings.Solution);

                info.CreateNoWindow = true;

                var ret = Processes.Execute(info, writer, writer);
                return ret == 0;
            }, progress);
        }

        private string GetWorkingDirectory(BuildRequest settings)
        {
            return Path.Combine(Path.GetDirectoryName(settings.Solution), "bin", "gc");
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

        private bool RunTests(BuildRequest settings, IProgress<string> progress)
        {
            if (!settings.RunTests)
            {
                return true;
            }

            return ExecuteWithOutputRedirection(writer =>
                {
                    var runner = new TestRunner
                    {
                        NUnitConsole = settings.TestRunnerExecutable,
                        WithGui = false,
                        Assemblies = settings.TestAssemblyPattern,
                    };

                    var stdOut = Console.Out;
                    Console.SetOut(writer);
                    runner.ExecuteEmbedded(GetWorkingDirectory(settings), writer, writer);
                    Console.SetOut(stdOut);

                    return runner.Succeeded;
                }, progress);
        }

        private bool CheckIn(BuildRequest settings, IProgress<string> progress)
        {
            if (!settings.CheckIn)
            {
                return true;
            }

            if (string.IsNullOrEmpty(settings.CheckInComment))
            {
                progress.Report("!! NO CHECKIN COMMENT PROVIDED !!");
                return false;
            }

            try
            {
                myGitService.Commit(settings.RepositoryRoot, settings.Files, settings.CheckInComment, settings.UserName, settings.UserEMail);
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
