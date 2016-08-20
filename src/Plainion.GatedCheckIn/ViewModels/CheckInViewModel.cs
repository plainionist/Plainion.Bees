using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
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
        private FileSystemWatcher myPendingChangesWatcher;

        [ImportingConstructor]
        public CheckInViewModel( BuildService buildService, GitService gitService )
        {
            myBuildService = buildService;
            myGitService = gitService;

            Files = new ObservableCollection<RepositoryEntry>();

            RefreshCommand = new DelegateCommand( OnRefresh );
            DiffToPreviousCommand = new DelegateCommand( OnDiffToPrevious, CanDiffToPrevious );

            buildService.BuildDefinitionChanged += OnBuildDefinitionChanged;
            OnBuildDefinitionChanged();
        }

        private void StartPendingChangesWatcher()
        {
            Contract.Invariant( myPendingChangesWatcher == null, "Pending changes watcher still running" );

            myPendingChangesWatcher = new FileSystemWatcher();
            myPendingChangesWatcher.Path = myBuildService.BuildDefinition.RepositoryRoot;
            myPendingChangesWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.DirectoryName;
            myPendingChangesWatcher.Filter = "*.*";
            myPendingChangesWatcher.IncludeSubdirectories = true;
            myPendingChangesWatcher.Created += OnChanged;
            myPendingChangesWatcher.Changed += OnChanged;
            myPendingChangesWatcher.Deleted += OnChanged;
            myPendingChangesWatcher.Renamed += OnChanged;

            myPendingChangesWatcher.EnableRaisingEvents = true;
        }

        private void OnChanged( object source, FileSystemEventArgs e )
        {
            Debug.WriteLine( "Workspace change detected" );

            Application.Current.Dispatcher.BeginInvoke( new Action( () => UpdateFiles() ) );
        }

        private void StopPendingChangesWatcher()
        {
            if( myPendingChangesWatcher != null )
            {
                myPendingChangesWatcher.Dispose();
            }
        }

        private void OnBuildDefinitionChanged()
        {
            if( BuildDefinition != null )
            {
                BuildDefinition.PropertyChanged -= BuildDefinition_PropertyChanged;
            }

            StopPendingChangesWatcher();

            BuildDefinition = myBuildService.BuildDefinition;

            if( BuildDefinition != null )
            {
                BuildDefinition.PropertyChanged += BuildDefinition_PropertyChanged;
                OnRepositoryRootChanged();
            }

            DiffToPreviousCommand.RaiseCanExecuteChanged();
        }

        private void BuildDefinition_PropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            if( e.PropertyName == PropertySupport.ExtractPropertyName( () => BuildDefinition.RepositoryRoot ) )
            {
                OnRepositoryRootChanged();
            }
            else if( e.PropertyName == PropertySupport.ExtractPropertyName( () => BuildDefinition.DiffTool ) )
            {
                DiffToPreviousCommand.RaiseCanExecuteChanged();
            }

            OnPropertyChanged( e.PropertyName );
        }

        private void OnRepositoryRootChanged()
        {
            StopPendingChangesWatcher();

            if( !string.IsNullOrEmpty( BuildDefinition.RepositoryRoot ) && Directory.Exists( BuildDefinition.RepositoryRoot ) )
            {
                StartPendingChangesWatcher();
                UpdateFiles();
            }
        }

        public BuildDefinition BuildDefinition { get; private set; }

        public async void UpdateFiles()
        {
            Debug.WriteLine( "Updating pending changes" );

            Files.Clear();

            var entries = await myGitService.GetChangedAndNewFilesAsync( BuildDefinition.RepositoryRoot );

            // workaround: FileSystemWatcher may trigger this method here several times before first task returned
            // this has to be fixed to lower load on the system
            Files.Clear();

            var files = entries
                .Select( e => new RepositoryEntry( e ) { IsChecked = true } )
                .OrderBy( e => e.File );

            Files.AddRange( files );
        }

        public ObservableCollection<RepositoryEntry> Files { get; private set; }

        public RepositoryEntry SelectedFile
        {
            get { return mySelectedFile; }
            set { SetProperty( ref mySelectedFile, value ); }
        }

        public string CheckInComment
        {
            get { return myCheckInComment; }
            set { SetProperty( ref myCheckInComment, value ); }
        }

        public ICommand RefreshCommand { get; private set; }

        public void OnRefresh()
        {
            UpdateFiles();
        }

        public DelegateCommand DiffToPreviousCommand { get; private set; }

        private bool CanDiffToPrevious()
        {
            return BuildDefinition != null && !string.IsNullOrEmpty( BuildDefinition.DiffTool );
        }

        public void OnDiffToPrevious()
        {
            var headFile = myGitService.GetHeadOf( BuildDefinition.RepositoryRoot, SelectedFile.File );

            var parts = Regex.Matches( BuildDefinition.DiffTool, @"[\""].+?[\""]|[^ ]+" )
                            .Cast<Match>()
                            .Select( m => m.Value )
                            .ToList();

            var executable = parts.First().Trim( '"' );
            var args = string.Join( " ", parts.Skip( 1 ) )
                .Replace( "%base", headFile )
                .Replace( "%mine", Path.Combine( BuildDefinition.RepositoryRoot, SelectedFile.File ) );

            // "C:\Program Files\TortoiseHg\kdiff3.exe" %base %mine
            Process.Start( executable, args );
        }
    }
}
