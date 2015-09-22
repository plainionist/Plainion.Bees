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
        private GitService myGitService;

        [ImportingConstructor]
        public WorkflowService(GitService gitService)
        {
            myGitService = gitService;
        }

        public Task<bool> ExecuteAsync(CheckInRequest settings, IProgress<string> progress)
        {
            return Task<bool>.Run(() =>
            {
                return BuildSolution(settings, progress)
                       && RunTests(settings, progress)
                       && CheckIn(settings, progress);
            });
        }

        private bool BuildSolution(CheckInRequest settings, IProgress<string> progress)
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

        private string GetWorkingDirectory(CheckInRequest settings)
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

        class ProducerConsumerStream : Stream
        {
            private readonly MemoryStream myInnerStream;
            private long myReadPosition;
            private long myWritePosition;

            public ProducerConsumerStream()
            {
                myInnerStream = new MemoryStream();
            }

            public override bool CanRead { get { return true; } }

            public override bool CanSeek { get { return false; } }

            public override bool CanWrite { get { return true; } }

            public override void Flush()
            {
                lock (myInnerStream)
                {
                    myInnerStream.Flush();
                }
            }

            public override long Length
            {
                get
                {
                    lock (myInnerStream)
                    {
                        return myInnerStream.Length;
                    }
                }
            }

            public override long Position
            {
                get { throw new NotSupportedException(); }
                set { throw new NotSupportedException(); }
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                lock (myInnerStream)
                {
                    myInnerStream.Position = myReadPosition;
                    int red = myInnerStream.Read(buffer, offset, count);
                    myReadPosition = myInnerStream.Position;

                    return red;
                }
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotImplementedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                lock (myInnerStream)
                {
                    myInnerStream.Position = myWritePosition;
                    myInnerStream.Write(buffer, offset, count);
                    myWritePosition = myInnerStream.Position;
                }
            }
        }

        private bool RunTests(CheckInRequest settings, IProgress<string> progress)
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

        private bool CheckIn(CheckInRequest settings, IProgress<string> progress)
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
                myGitService.Commit(settings.RepositoryRoot, settings.Files, settings.CheckInComment);
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
