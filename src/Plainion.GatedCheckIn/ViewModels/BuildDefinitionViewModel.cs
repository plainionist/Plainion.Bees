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
    class BuildDefinitionViewModel : BindableBase
    {
        private GitService myGitService;
        private bool? mySucceeded;

        [ImportingConstructor]
        public BuildDefinitionViewModel(BuildService buildService, GitService gitService)
        {
            myGitService = gitService;

            BuildDefinition = buildService.BuildDefinition;
            BuildDefinition.PropertyChanged += BuildDefinition_PropertyChanged;
            Configurations = new[] { "Debug", "Release" };
            Platforms = new[] { "Any CPU", "x86", "x64" };

            RefreshCommand = new DelegateCommand(OnRefresh);
        }

        private void BuildDefinition_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == PropertySupport.ExtractPropertyName(() => BuildDefinition.RepositoryRoot))
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
        }

        public BuildDefinition BuildDefinition { get; private set; }

        private async void UpdateFiles()
        {
            Files.Clear();

            var entries = await myGitService.GetChangedAndNewFilesAsync(BuildDefinition.RepositoryRoot);

            var files = entries
                .Select(e => new RepositoryEntry(e) { IsChecked = true })
                .OrderBy(e => e.File);

            Files.AddRange(files);
        }

        public ObservableCollection<RepositoryEntry> Files { get; private set; }

        public DelegateCommand GoCommand { get; private set; }

        public bool? Succeeded
        {
            get { return mySucceeded; }
            set { SetProperty(ref mySucceeded, value); }
        }

        public IEnumerable<string> Configurations { get; private set; }

        public IEnumerable<string> Platforms { get; private set; }

        public ICommand RefreshCommand { get; private set; }

        public void OnRefresh()
        {
            UpdateFiles();
        }
    }
}
