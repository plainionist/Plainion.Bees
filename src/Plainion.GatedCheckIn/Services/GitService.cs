using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace Plainion.GatedCheckIn.Services
{
    [Export]
    class GitService
    {
        public Task<IEnumerable<StatusEntry>> GetChangedAndNewFilesAsync(string repositoryRoot)
        {
            return Task<IEnumerable<StatusEntry>>.Run(() =>
            {
                using (var repo = new Repository(repositoryRoot))
                {
                    return (IEnumerable<StatusEntry>)repo.RetrieveStatus()
                        .Where(e => (e.State & FileStatus.Ignored) == 0)
                        .ToList();
                }
            });
        }
    }
}
