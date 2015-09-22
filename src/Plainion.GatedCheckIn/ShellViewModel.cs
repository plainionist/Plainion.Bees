using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;

using Plainion.Collections;
using Plainion.GatedCheckIn.Services;
using Plainion.Windows;

namespace Plainion.GatedCheckIn
{
    [Export]
    class ShellViewModel : BindableBase
    {
        private WorkflowService myWorkflowService;
        private GitService myGitService;
        private string myRepositoryRoot;
        private string mySolution;
        private bool myRunTests;
        private bool myCheckIn;
        private bool myIsBusy;
        private bool? mySucceeded;
        private string myConfiguration;
        private string myPlatform;
        private string myTestRunnerExecutable;
        private string myTestAssemblyPattern;

        [ImportingConstructor]
        public ShellViewModel(WorkflowService workflowService, GitService gitService)
        {
            myWorkflowService = workflowService;
            myGitService = gitService;

            GoCommand = new DelegateCommand(OnGo, CanGo);
            Messages = new ObservableCollection<string>();

            Files = new ObservableCollection<RepositoryEntry>();

            Configurations = new[] { "Debug", "Release" };
            Configuration = Configurations.First();

            Platforms = new[] { "Any CPU", "x86", "x64" };
            Platform = Platforms.First();

            RunTests = true;
            CheckIn = true;

            TestAssemblyPattern = "*Tests.dll";
            TestRunnerExecutable = @"\Extern\NUnit\bin\nunit-console.exe";

            RefreshCommand = new DelegateCommand(OnRefresh);

            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                RepositoryRoot = Path.GetFullPath(args[1]);
            }
        }

        public string RepositoryRoot
        {
            get { return myRepositoryRoot; }
            set
            {
                if (SetProperty(ref myRepositoryRoot, value))
                {
                    if (string.IsNullOrEmpty(myRepositoryRoot) || !Directory.Exists(myRepositoryRoot))
                    {
                        return;
                    }

                    var solutionPath = Directory.GetFiles(myRepositoryRoot, "*.sln", SearchOption.TopDirectoryOnly)
                            .FirstOrDefault();
                    if (solutionPath != null)
                    {
                        Solution = Path.GetFileName(solutionPath);
                    }

                    UpdateFiles();
                }
            }
        }

        private async void UpdateFiles()
        {
            Files.Clear();

            var entries = await myGitService.GetChangedAndNewFilesAsync(myRepositoryRoot);

            var files = entries
                .Select(e => new RepositoryEntry(e) { IsChecked = true })
                .OrderBy(e => e.File);

            Files.AddRange(files);
        }

        public ObservableCollection<RepositoryEntry> Files { get; private set; }

        public string Solution
        {
            get { return mySolution; }
            set { SetProperty(ref mySolution, value); }
        }

        public bool RunTests
        {
            get { return myRunTests; }
            set { SetProperty(ref myRunTests, value); }
        }

        public bool CheckIn
        {
            get { return myCheckIn; }
            set { SetProperty(ref myCheckIn, value); }
        }

        public DelegateCommand GoCommand { get; private set; }

        private bool CanGo() { return !myIsBusy; }

        private void OnGo()
        {
            Messages.Clear();
            myIsBusy = true;
            GoCommand.RaiseCanExecuteChanged();

            var progress = new Progress<string>(p => Messages.Add(p));

            var settings = new Settings
            {
                Solution = Path.Combine(RepositoryRoot, Solution),
                RunTests = RunTests,
                CheckIn = CheckIn,
                Configuration = Configuration,
                Platform = Platform,
                TestRunnerExecutable = TestRunnerExecutable,
                TestAssemblyPattern = TestAssemblyPattern
            };

            myWorkflowService.ExecuteAsync(settings, progress)
                .RethrowExceptionsInUIThread()
                .ContinueWith(t =>
                    {
                        Succeeded = t.Result;

                        myIsBusy = false;
                        GoCommand.RaiseCanExecuteChanged();
                    }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public ObservableCollection<string> Messages { get; private set; }

        public bool? Succeeded
        {
            get { return mySucceeded; }
            set { SetProperty(ref mySucceeded, value); }
        }

        public IEnumerable<string> Configurations { get; private set; }

        public string Configuration
        {
            get { return myConfiguration; }
            set { SetProperty(ref myConfiguration, value); }
        }

        public IEnumerable<string> Platforms { get; private set; }

        public string Platform
        {
            get { return myPlatform; }
            set { SetProperty(ref myPlatform, value); }
        }

        public string TestRunnerExecutable
        {
            get { return myTestRunnerExecutable; }
            set { SetProperty(ref myTestRunnerExecutable, value); }
        }

        public string TestAssemblyPattern
        {
            get { return myTestAssemblyPattern; }
            set { SetProperty(ref myTestAssemblyPattern, value); }
        }

        public ICommand RefreshCommand { get; private set; }

        public void OnRefresh()
        {
            UpdateFiles();
        }
    }
}
