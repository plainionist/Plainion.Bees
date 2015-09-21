using Microsoft.Practices.Prism.Mvvm;

namespace Plainion.GatedCheckIn
{
    class RepositoryEntry : BindableBase
    {
        private bool myIsChecked;

        public RepositoryEntry(string file)
        {
            File = file;
        }

        public string File { get; private set; }

        public bool IsChecked
        {
            get { return myIsChecked; }
            set { SetProperty(ref myIsChecked, value); }
        }
    }
}
