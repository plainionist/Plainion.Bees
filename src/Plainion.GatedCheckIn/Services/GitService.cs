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
        public Task<IEnumerable<string>> GetChangedAndNewFilesAsync(string repositoryRoot)
        {
            return Task<IEnumerable<string>>.Run(() =>
            {
                using (var repo = new Repository(repositoryRoot))
                {
                    return (IEnumerable<string>)repo.RetrieveStatus()
                        .Where(e => (e.State & FileStatus.Ignored) == 0)
                        .Select(e => e.FilePath)
                        .ToList();
                }
            });
        }
    }
}
