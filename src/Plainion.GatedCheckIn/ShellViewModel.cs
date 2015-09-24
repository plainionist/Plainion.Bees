using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Mvvm;
using Plainion.GatedCheckIn.ViewModels;

namespace Plainion.GatedCheckIn
{
    [Export]
    class ShellViewModel : BindableBase
    {
        [Import]
        public BuildViewModel BuildViewModel { get; private set; }
    }
}
