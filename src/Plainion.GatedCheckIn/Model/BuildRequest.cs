using System;
using System.Collections.Generic;

namespace Plainion.GatedCheckIn.Model
{
    [Serializable]
    class BuildRequest
    {
        public string CheckInComment { get; set; }

        public IReadOnlyCollection<string> Files { get; set; }

        public string Password { get; set; }
    }
}
