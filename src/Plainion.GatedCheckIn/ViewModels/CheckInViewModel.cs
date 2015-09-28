using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Plainion.Collections;
using Plainion.GatedCheckIn.Model;
using Plainion.GatedCheckIn.Services;

namespace Plainion.GatedCheckIn.ViewModels
{
    [Export]
    class CheckInViewModel : BindableBase
    {
        private BuildService myBuildService;
        private GitService myGitService;
        private RepositoryEntry mySelectedFile;
        private string myCheckInComment;
        private string myUserName;
        private string myUserEMail;

        [ImportingConstructor]
        public CheckInViewModel(BuildService buildService, GitService gitService)
        {
            myBuildService = buildService;
            myGitService = gitService;

            Files = new ObservableCollection<RepositoryEntry>();

            Configurations = new[] { "Debug", "Release" };
            Platforms = new[] { "Any CPU", "x86", "x64" };

            RefreshCommand = new DelegateCommand(OnRefresh);
            DiffToPreviousCommand = new DelegateCommand(OnDiffToPrevious);

            buildService.BuildDefinitionChanged += OnBuildDefinitionChanged;
            OnBuildDefinitionChanged();
        }

        private void OnBuildDefinitionChanged()
        {
            if (BuildDefinition != null)
            {
                BuildDefinition.PropertyChanged -= BuildDefinition_PropertyChanged;
            }

            BuildDefinition = myBuildService.BuildDefinition;

            if (BuildDefinition != null)
            {
                BuildDefinition.PropertyChanged += BuildDefinition_PropertyChanged;
                OnRepositoryRootChanged();
            }
        }

        private void BuildDefinition_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == PropertySupport.ExtractPropertyName(() => BuildDefinition.RepositoryRoot))
            {
                OnRepositoryRootChanged();
            }

            OnPropertyChanged(e.PropertyName);
        }

        private void OnRepositoryRootChanged()
        {
            if (!string.IsNullOrEmpty(BuildDefinition.RepositoryRoot) && Directory.Exists(BuildDefinition.RepositoryRoot))
            {
                var solutionPath = Directory.GetFiles(BuildDefinition.RepositoryRoot, "*.sln", SearchOption.TopDirectoryOnly)
                    .FirstOrDefault();
                if (solutionPath != null)
                {
                    BuildDefinition.Solution = Path.GetFileName(solutionPath);
                }

                UpdateFiles();
            }
        }

        public BuildDefinition BuildDefinition { get; private set; }

        public async void UpdateFiles()
        {
            Files.Clear();

            var entries = await myGitService.GetChangedAndNewFilesAsync(BuildDefinition.RepositoryRoot);

            var files = entries
                .Select(e => new RepositoryEntry(e) { IsChecked = true })
                .OrderBy(e => e.File);

            Files.AddRange(files);
        }

        public ObservableCollection<RepositoryEntry> Files { get; private set; }

        public RepositoryEntry SelectedFile
        {
            get { return mySelectedFile; }
            set { SetProperty(ref mySelectedFile, value); }
        }
        
        public IEnumerable<string> Configurations { get; private set; }

        public IEnumerable<string> Platforms { get; private set; }

        public string CheckInComment
        {
            get { return myCheckInComment; }
            set { SetProperty(ref myCheckInComment, value); }
        }

        public string UserName
        {
            get { return myUserName; }
            set { SetProperty(ref myUserName, value); }
        }

        public string UserEMail
        {
            get { return myUserEMail; }
            set { SetProperty(ref myUserEMail, value); }
        }

        public ICommand RefreshCommand { get; private set; }

        public void OnRefresh()
        {
            UpdateFiles();
        }

        public ICommand DiffToPreviousCommand { get; private set; }

        public void OnDiffToPrevious()
        {
            myGitService.GetLatest(BuildDefinition.RepositoryRoot, SelectedFile.File);
        }
    }
}
