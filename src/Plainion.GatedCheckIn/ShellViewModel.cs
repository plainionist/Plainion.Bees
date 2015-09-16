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
using Plainion.GatedCheckIn.Services;
using Plainion.Windows;

namespace Plainion.GatedCheckIn
{
    [Export]
    class ShellViewModel : BindableBase
    {
        private WorkflowService myWorkflowService;
        private string mySolution;
        private bool myRunTests;
        private bool myCheckIn;
        private bool myIsBusy;
        private bool? mySucceeded;
        private string myConfiguration;
        private string myPlatform;

        [ImportingConstructor]
        public ShellViewModel(WorkflowService workflowService)
        {
            myWorkflowService = workflowService;

            GoCommand = new DelegateCommand(OnGo, CanGo);
            Messages = new ObservableCollection<string>();

            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                Solution = Path.GetFullPath(args[1]);
            }

            Configurations = new[] { "Debug", "Release" };
            Configuration = Configurations.First();

            Platforms = new[] { "Any CPU", "x86", "x64" };
            Platform = Platforms.First();
        }

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
                Solution = Solution,
                RunTests = RunTests,
                CheckIn = CheckIn,
                Configuration = Configuration,
                Platform = Platform
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
    }
}
