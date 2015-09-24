using LibGit2Sharp;
using Microsoft.Practices.Prism.Mvvm;

namespace Plainion.GatedCheckIn.ViewModels
{
    class RepositoryEntry : BindableBase
    {
        private StatusEntry myEntry;
        private bool myIsChecked;

        public RepositoryEntry(StatusEntry entry)
        {
            Contract.RequiresNotNull(entry, "entry");

            myEntry = entry;
        }

        public string File { get { return myEntry.FilePath; } }

        public FileStatus State { get { return myEntry.State; } }

        public bool IsChecked
        {
            get { return myIsChecked; }
            set { SetProperty(ref myIsChecked, value); }
        }
    }
}
