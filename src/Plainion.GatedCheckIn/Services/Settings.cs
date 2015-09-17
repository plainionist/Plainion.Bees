
namespace Plainion.GatedCheckIn.Services
{
    class Settings
    {
        public string Solution { get; set; }
        public bool RunTests { get; set; }
        public bool CheckIn { get; set; }
        public string Configuration { get; set; }
        public string Platform { get; set; }
        public string TestRunnerExecutable { get; set; }
        public string TestAssemblyPattern { get; set; }
    }
}
