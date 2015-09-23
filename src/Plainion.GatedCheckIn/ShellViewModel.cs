﻿using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Plainion.GatedCheckIn.Services;
using Plainion.GatedCheckIn.ViewModels;
using Plainion.Windows;

namespace Plainion.GatedCheckIn
{
    [Export]
    class ShellViewModel : BindableBase
    {
        private BuildService myBuildService;
        private bool myIsBusy;
        private bool? mySucceeded;
        private string myLog;

        [ImportingConstructor]
        public ShellViewModel(BuildService buildService, GitService gitService)
        {
            myBuildService = buildService;

            GoCommand = new DelegateCommand(OnGo, CanGo);

            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                myBuildService.InitializeBuildDefinition(Path.Combine(args[1], Path.GetFileName(args[1]) + ".gc"));
            }

            // needs to be done after initialization of the BuildDefinition
            BuildDefinitionViewModel = new BuildDefinitionViewModel(buildService, gitService);
        }

        public BuildDefinitionViewModel BuildDefinitionViewModel { get; private set; }

        public DelegateCommand GoCommand { get; private set; }

        private bool CanGo() { return !myIsBusy; }

        private void OnGo()
        {
            Log = null;
            myIsBusy = true;
            Succeeded = null;
            GoCommand.RaiseCanExecuteChanged();

            var progress = new Progress<string>(p => Log += p + Environment.NewLine);

            var request = new BuildRequest
            {
                CheckInComment = BuildDefinitionViewModel.CheckInComment,
                UserName = BuildDefinitionViewModel.UserName,
                UserEMail = BuildDefinitionViewModel.UserEMail,
                Files = BuildDefinitionViewModel.Files
                    .Where(e => e.IsChecked)
                    .Select(e => e.File)
                    .ToList(),
            };

            myBuildService.ExecuteAsync(request, progress)
                .RethrowExceptionsInUIThread()
                .ContinueWith(t =>
                    {
                        Succeeded = t.Result;

                        myIsBusy = false;
                        GoCommand.RaiseCanExecuteChanged();

                        BuildDefinitionViewModel.UpdateFiles();
                    }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public string Log
        {
            get { return myLog; }
            set { SetProperty(ref myLog, value); }
        }

        public bool? Succeeded
        {
            get { return mySucceeded; }
            set { SetProperty(ref mySucceeded, value); }
        }
    }
}
