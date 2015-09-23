using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

using Plainion.GatedCheckIn.ViewModels;


namespace Plainion.GatedCheckIn.Views
{
    [Export]
    public partial class BuildDefinitionView : UserControl
    {
        [ImportingConstructor]
        internal BuildDefinitionView(BuildDefinitionViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}
