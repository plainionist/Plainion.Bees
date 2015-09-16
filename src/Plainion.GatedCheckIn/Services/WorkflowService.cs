using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Plainion.IO;

namespace Plainion.GatedCheckIn.Services
{
    [Export]
    class WorkflowService
    {
        public Task<bool> ExecuteAsync(Settings settings, IProgress<string> progress)
        {
            return Task<bool>.Run(() =>
                {
                    BuildSolution(settings.Solution, settings.Configuration, settings.Platform, progress);
                    return false;
                });
        }

        private void BuildSolution(string solution, string configuration, string platform, IProgress<string> progress)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            var reader = new StreamReader(stream);

            var cts = new CancellationTokenSource();
            var task = Task.Run(() =>
                {
                    while (!cts.Token.IsCancellationRequested)
                    {
                        var line = reader.ReadLine();
                        if (line != null)
                        {
                            progress.Report(line);
                        }
                    }
                }, cts.Token);

            var info = new ProcessStartInfo(@"C:\Program Files (x86)\MSBuild\12.0\Bin\MSBuild.exe",
                "/m /p:Configuration=" + configuration + " /p:Platform=\"" + platform + "\" " + solution);
            info.CreateNoWindow = true;

            try
            {
                Processes.Execute(info, writer, writer);
            }
            catch (Exception ex)
            {
                cts.Cancel();

                throw new Exception("Failed to build solution", ex);
            }
        }
    }
}
