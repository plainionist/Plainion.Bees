using System.Collections.Generic;

namespace Plainion.GatedCheckIn.Services
{
    class BuildRequest
    {
        public string CheckInComment { get; set; }
        public IReadOnlyCollection<string> Files { get; set; }
        public string UserName { get; set; }
        public string UserEMail { get; set; }
    }
}
