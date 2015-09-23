using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Plainion.GatedCheckIn.Model
{
    [DataContract(Namespace = "http://github.com/ronin4net/plainion/GatedCheckIn", Name = "BuildDefinition")]
    class BuildDefinition
    {
        public string RepositoryRoot { get; set; }
        public string Solution { get; set; }
        public bool RunTests { get; set; }
        public bool CheckIn { get; set; }
        public string Configuration { get; set; }
        public string Platform { get; set; }
        public string TestRunnerExecutable { get; set; }
        public string TestAssemblyPattern { get; set; }
        public string CheckInComment { get; set; }
        public IReadOnlyCollection<string> Files { get; set; }
        public string UserName { get; set; }
        public string UserEMail { get; set; }
    }
}
