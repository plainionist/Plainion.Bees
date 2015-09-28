using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Mvvm;

namespace Plainion.GatedCheckIn.ViewModels
{
    [Export]
    class BuildLogViewModel : BindableBase
    {
        private bool? mySucceeded;
        private string myLog;

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
