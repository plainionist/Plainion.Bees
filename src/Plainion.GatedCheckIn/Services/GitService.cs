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

        internal bool Commit(IEnumerable<string> files)
        {
            /*
            using (var repo = new Repository("path/to/your/repo"))
            {
                // Write content to file system
                var content = "Commit this!";
                File.WriteAllText(Path.Combine(repo.Info.WorkingDirectory, "fileToCommit.txt"), content);

                // Stage the file
                repo.Stage("fileToCommit.txt");

                // Create the committer's signature and commit
                Signature author = new Signature("James", "@jugglingnutcase", DateTime.Now);
                Signature committer = author;

                // Commit to the repository
                Commit commit = repo.Commit("Here's a commit i made!", author, committer);
            }
             * */
            return false;
        }
    }
}
