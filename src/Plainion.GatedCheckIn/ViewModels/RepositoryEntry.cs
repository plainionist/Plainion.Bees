using LibGit2Sharp;
using Microsoft.Practices.Prism.Mvvm;
using Plainion.GatedCheckIn.Services.SourceControl;

namespace Plainion.GatedCheckIn.ViewModels
{
    class RepositoryEntry : BindableBase
    {
        private Change myEntry;
        private bool myIsChecked;

        public RepositoryEntry( Change entry )
        {
            Contract.RequiresNotNull( entry, "entry" );

            myEntry = entry;
        }

        public string File { get { return myEntry.Path; } }

        public ChangeType State { get { return myEntry.ChangeType; } }

        public bool IsChecked
        {
            get { return myIsChecked; }
            set { SetProperty( ref myIsChecked, value ); }
        }
    }
}
