using System.Collections.Generic;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace Plainion.GatedCheckIn.Services
{
    interface ISourceControl
    {
        Task<IEnumerable<StatusEntry>> GetChangedAndNewFilesAsync( string workspaceRoot );

        void Commit( string workspaceRoot, IEnumerable<string> files, string comment, string name, string email );

        void Push( string workspaceRoot, string name, string password );
        
        void Revert( string workspaceRoot, string file );

        void DiffToPrevious( string workspaceRoot, string file, string diffTool );
    }
}
