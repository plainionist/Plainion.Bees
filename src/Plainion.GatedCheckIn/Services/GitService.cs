using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace Plainion.GatedCheckIn.Services
{
    /// <summary>
    /// Thread-safe and re-entrent
    /// </summary>
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
                foreach (var file in files)
                {
                    repo.Stage(file);
                }

                var author = new Signature(name, email, DateTime.Now);

                repo.Commit(comment, author, author);
            }
        }

        /// <summary>
        /// Returns path to a temp file which contains the HEAD version of the given file.
        /// The caller has to take care to delete the file.
        /// </summary>
        public string GetHeadOf(string repositoryRoot, string relativePath)
        {
            using (var repo = new Repository(repositoryRoot))
            {
                var log = repo.Commits.QueryBy(relativePath);
                if (log == null || !log.Any())
                {
                    // file not yet tracked -> ignore
                    return null;
                }

                var head = log.First();
                var treeEntry = head.Commit.Tree[relativePath];
                var blob = (Blob)treeEntry.Target;

                var file = Path.Combine(Path.GetTempPath(), Path.GetFileName(relativePath) + ".head");

                using (var reader = new StreamReader(blob.GetContentStream()))
                {
                    using (var writer = new StreamWriter(file))
                    {
                        while (!reader.EndOfStream)
                        {
                            writer.WriteLine(reader.ReadLine());
                        }
                    }
                }

                return file;
            }
        }
    }
}
