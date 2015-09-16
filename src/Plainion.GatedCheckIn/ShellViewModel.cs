using System;
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

            myWorkflowService.ExecuteAsync(Solution, RunTests, CheckIn, progress)
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
    }
}
