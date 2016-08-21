using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Text.RegularExpressions;
using System.Windows.Input;
using LibGit2Sharp;
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
        private ISourceControl myGitService;
        private RepositoryEntry mySelectedFile;
        private string myCheckInComment;
        private PendingChangesObserver myPendingChangesObserver;

        [ImportingConstructor]
        public CheckInViewModel( BuildService buildService, ISourceControl gitService )
        {
            myBuildService = buildService;
            myGitService = gitService;

            Files = new ObservableCollection<RepositoryEntry>();

            RefreshCommand = new DelegateCommand( RefreshPendingChanges );
            RevertCommand = new DelegateCommand<string>( OnRevert );
            DiffToPreviousCommand = new DelegateCommand( OnDiffToPrevious, CanDiffToPrevious );

            myPendingChangesObserver = new PendingChangesObserver( myGitService, OnPendingChangesChanged );

            buildService.BuildDefinitionChanged += OnBuildDefinitionChanged;
            OnBuildDefinitionChanged();
        }

        private void OnPendingChangesChanged( IEnumerable<StatusEntry> pendingChanges )
        {
            Debug.WriteLine( "Updating pending changes" );

            Files.Clear();

            var files = pendingChanges
                .Select( e => new RepositoryEntry( e ) { IsChecked = true } )
                .OrderBy( e => e.File );

            Files.AddRange( files );
        }

        private void OnBuildDefinitionChanged()
        {
            if( BuildDefinition != null )
            {
                BuildDefinition.PropertyChanged -= BuildDefinition_PropertyChanged;
            }

            myPendingChangesObserver.Stop();

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
            myPendingChangesObserver.Stop();

            if( !string.IsNullOrEmpty( BuildDefinition.RepositoryRoot ) && Directory.Exists( BuildDefinition.RepositoryRoot ) )
            {
                myPendingChangesObserver.Start( myBuildService.BuildDefinition.RepositoryRoot );
            }
        }

        public BuildDefinition BuildDefinition { get; private set; }

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

        public SecureString SecurePassword { get; set; }

        public ICommand RefreshCommand { get; private set; }

        public async void RefreshPendingChanges()
        {
            if( string.IsNullOrEmpty( BuildDefinition.RepositoryRoot ) || !Directory.Exists( BuildDefinition.RepositoryRoot ) )
            {
                return;
            }

            var pendingChanges = await myGitService.GetChangedAndNewFilesAsync( BuildDefinition.RepositoryRoot );

            OnPendingChangesChanged( pendingChanges );
        }

        public ICommand RevertCommand { get; private set; }

        private void OnRevert( string file )
        {
            Debug.WriteLine( "Reverting changes on '" + file + "'" );

            myGitService.Revert( BuildDefinition.RepositoryRoot, file );
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
