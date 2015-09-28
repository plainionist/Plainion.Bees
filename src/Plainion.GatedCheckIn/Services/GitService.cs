using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
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
                foreach (var file in files)
                {
                    repo.Stage(file);
                }

                var author = new Signature(name, email, DateTime.Now);

                repo.Commit(comment, author, author);
            }
        }

        public void GetLatest(string repositoryRoot, string relativePath)
        {
            // C:\Program Files\TortoiseHg\kdiff3.exe %1 -fname %6 %2 -fname %7
            using (var repo = new Repository(repositoryRoot))
            {
                var log = repo.Commits.QueryBy(relativePath);
                if (log == null || !log.Any())
                {
                    // file not yet tracked -> ignore
                    return;
                }

                var head = log.First();
                var treeEntry = head.Commit.Tree.Single(e => e.Path == relativePath);
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

                Process.Start("kdiff3", file + " " + Path.Combine(repositoryRoot, relativePath));
            }
        }
    }
}
