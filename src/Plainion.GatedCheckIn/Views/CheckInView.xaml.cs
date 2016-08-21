using System.Windows;
using System.Windows.Controls;

namespace Plainion.GatedCheckIn.Views
{
    public partial class CheckInView : UserControl
    {
        public CheckInView()
        {
            InitializeComponent();
        }

        // http://stackoverflow.com/questions/1483892/how-to-bind-to-a-passwordbox-in-mvvm
        private void PasswordBox_PasswordChanged( object sender, RoutedEventArgs e )
        {
            if( DataContext != null )
            { ( ( dynamic )DataContext ).SecurePassword = ( ( PasswordBox )sender ).SecurePassword; }
        }
    }
}
