using System.Windows;

namespace Plainion.WpfBrushes
{
    public partial class Shell : Window
    {
        public Shell()
        {
            InitializeComponent();

            DataContext = new ShellViewModel();
        }
    }
}
