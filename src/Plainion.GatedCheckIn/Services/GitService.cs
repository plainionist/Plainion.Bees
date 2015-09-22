using System;
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

        public void Commit(string repositoryRoot, IEnumerable<string> files, string comment, string name, string email)
        {
            using (var repo = new Repository(repositoryRoot))
            {
                //repo.Stage("fileToCommit.txt");

                var author = new Signature(name, email, DateTime.Now);

                repo.Commit(comment, author, author);
            }
        }
    }
}
