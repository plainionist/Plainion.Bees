using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;

namespace Plainion.GatedCheckIn.Services
{
    [Export]
    class WorkflowService
    {
        public Task<bool> ExecuteAsync(string Solution, bool RunTests, bool CheckIn, IProgress<string> progress)
        {
            return Task<bool>.Run(() =>
                {
                    Thread.Sleep(10 * 1000);
                    return false;
                });
        }
    }
}
