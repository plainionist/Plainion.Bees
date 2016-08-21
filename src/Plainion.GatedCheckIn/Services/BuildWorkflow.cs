using System;
using System.IO;
using System.Threading.Tasks;
using Plainion.GatedCheckIn.Model;
using Plainion.GatedCheckIn.Services.SourceControl;
using Plainion.Scripts.TestRunner;

namespace Plainion.GatedCheckIn.Services
{
    internal class BuildWorkflow
    {
        private ISourceControl mySourceControl;
        private BuildDefinition myDefinition;
        private BuildRequest myRequest;

        public BuildWorkflow( ISourceControl sourceControl, BuildDefinition definition, BuildRequest request )
        {
            mySourceControl = sourceControl;
            myDefinition = definition;
            myRequest = request;
        }

        internal Task<bool> ExecuteAsync( IProgress<string> progress )
        {
            // clone thread save copy of the relevant paramters;
            myDefinition = Objects.Clone( myDefinition );
            myRequest = Objects.Clone( myRequest );

            return Task<bool>.Run( () =>
                Execute( "build", p => ExecuteMsbuildScript( Path.Combine( myDefinition.RepositoryRoot, myDefinition.Solution ), p ), progress )
                && ( !myDefinition.RunTests || Execute( "test", RunTests, progress ) )
                && ( !myDefinition.CheckIn || Execute( "checkin", CheckIn, progress ) )
                && ( !myDefinition.Push || Execute( "push", Push, progress ) ) );
        }

        private bool Execute( string activity, Func<IProgress<string>, bool> action, IProgress<string> progress )
        {
            try
            {
                var success = action( progress );

                if ( success )
                {
                    progress.Report( "--- " + activity.ToUpper() + " SUCCEEDED ---" );
                }
                else
                {
                    progress.Report( "--- " + activity.ToUpper() + " FAILED ---" );
                }

                return success;
            }
            catch ( Exception ex )
            {
                progress.Report( "ERROR: " + ex.Message );
                progress.Report( "--- " + activity.ToUpper() + " FAILED ---" );
                return false;
            }
        }

        private bool ExecuteMsbuildScript( string script, IProgress<string> progress )
        {
            var process = new UiShellCommand( @"C:\Program Files (x86)\MSBuild\12.0\Bin\MSBuild.exe", progress );

            process.Execute(
                "/m",
                "/p:Configuration=" + myDefinition.Configuration,
                "/p:Platform=\"" + myDefinition.Platform + "\"",
                "/p:OutputPath=\"" + GetWorkingDirectory() + "\"",
                script );

            return process.ExitCode == 0;
        }

        private string GetWorkingDirectory()
        {
            return Path.Combine( myDefinition.RepositoryRoot, "bin", "gc" );
        }

        private bool RunTests( IProgress<string> progress )
        {
            Contract.Requires( File.Exists( myDefinition.TestRunnerExecutable ), "Runner executable not found: {0}", myDefinition.TestRunnerExecutable );

            var runner = new TestRunner
            {
                Assemblies = myDefinition.TestAssemblyPattern,
                WorkingDirectory = GetWorkingDirectory()
            };

            var nunitProject = runner.GenerateProject();

            if ( nunitProject == null )
            {
                progress.Report( "!! NO TEST ASSEMBLIES FOUND (check build definition; change test assembly pattern or disable test execution) !!" );

                return false;
            }

            var process = new UiShellCommand( myDefinition.TestRunnerExecutable, progress );

            // shadowcopy is an issue if we load files during UT according to assembly location
            process.Execute( "/noshadow " + nunitProject );

            return process.ExitCode == 0;
        }

        private bool CheckIn( IProgress<string> progress )
        {
            if ( string.IsNullOrEmpty( myRequest.CheckInComment ) )
            {
                progress.Report( "!! NO CHECKIN COMMENT PROVIDED !!" );
                return false;
            }

            mySourceControl.Commit( myDefinition.RepositoryRoot, myRequest.Files, myRequest.CheckInComment, myDefinition.UserName, myDefinition.UserEMail );

            return true;
        }

        private bool Push( IProgress<string> progress )
        {
            if ( myDefinition.UserPassword == null )
            {
                progress.Report( "!! NO PASSWORD PROVIDED !!" );
                return false;
            }

            mySourceControl.Push( myDefinition.RepositoryRoot, myDefinition.UserName, myDefinition.UserPassword.ToUnsecureString() );

            return true;
        }
    }
}
