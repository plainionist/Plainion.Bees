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
    class WorkflowService
    {
        public Task<bool> ExecuteAsync(Settings settings, IProgress<string> progress)
        {
            return Task<bool>.Run(() =>
                {
                    return BuildSolution(settings, progress)
                        && RunTests(settings, progress);
                });
        }

        private bool BuildSolution(Settings settings, IProgress<string> progress)
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

        private string GetWorkingDirectory(Settings settings)
        {
            return Path.Combine(Path.GetDirectoryName(settings.Solution), "bin", "gc");
        }

        private bool ExecuteWithOutputRedirection(Func<TextWriter, bool> Executor, IProgress<string> progress)
        {
            using (var stream = new MemoryStream())
            {
                using (var reader = StreamReader.Synchronized(new StreamReader(stream)))
                {
                    using (var writer = StreamWriter.Synchronized(new StreamWriter(stream)))
                    {
                        bool writeFinished=false;

                        var cts = new CancellationTokenSource();
                        var task = Task.Run(() =>
                        {
                            while (!writeFinished)
                            {
                                var line = reader.ReadLine();
                                if (line != null)
                                {
                                    progress.Report(line);
                                }
                            }
                        }, cts.Token);

                        try
                        {
                            return Executor(writer);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Failed to build solution", ex);
                        }
                        finally
                        {
                            writer.Flush();
                            stream.Flush();

                            Thread.Sleep(100);

                            writeFinished = true;

                            task.Wait();
                        }
                    }
                }
            }
        }

        private bool RunTests(Settings settings, IProgress<string> progress)
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
    }
}
