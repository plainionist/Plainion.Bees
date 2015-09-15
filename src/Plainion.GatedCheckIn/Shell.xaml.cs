using System.ComponentModel.Composition;
using System.Windows;

namespace Plainion.GatedCheckIn
{
    [Export]
    public partial class Shell : Window
    {
        [ImportingConstructor]
        internal Shell(ShellViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}
