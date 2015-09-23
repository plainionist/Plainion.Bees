using System.Collections.Generic;
using System.Runtime.Serialization;
using Plainion.Serialization;

namespace Plainion.GatedCheckIn.Model
{
    [DataContract(Namespace = "http://github.com/ronin4net/plainion/GatedCheckIn", Name = "BuildDefinition")]
    class BuildDefinition : SerializableBindableBase
    {
        [DataMember]
        public string RepositoryRoot { get; set; }

        [DataMember]
        public string Solution { get; set; }

        [DataMember]
        public bool RunTests { get; set; }

        [DataMember]
        public bool CheckIn { get; set; }

        [DataMember]
        public string Configuration { get; set; }

        [DataMember]
        public string Platform { get; set; }

        [DataMember]
        public string TestRunnerExecutable { get; set; }

        [DataMember]
        public string TestAssemblyPattern { get; set; }

        [DataMember]
        public string CheckInComment { get; set; }

        [DataMember]
        public IReadOnlyCollection<string> Files { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string UserEMail { get; set; }
    }
}
