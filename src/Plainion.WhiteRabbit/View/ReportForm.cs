using System;
using System.Windows.Forms;
using Plainion.WhiteRabbit.Presentation;

namespace Plainion.WhiteRabbit.View
{
    public partial class ReportForm : Form
    {
        private class Report
        {
            public Report( string name, Func<string> generator )
            {
                Name = name;
                Generator = generator;
            }

            public string Name { get; private set; }
            public Func<string> Generator { get; private set; }

            public override string ToString()
            {
                return Name;
            }
        }

        public ReportForm( Controller controller )
        {
            InitializeComponent();

            myReports.Items.Add( new Report( "Selected day", () => controller.GenerateDayReport( controller.CurrentDay ) ) );
            myReports.Items.Add( new Report( "Selected week", () => controller.GenerateWeekReport( controller.CurrentDay ) ) );

            myReports.SelectedIndex = 0;
            comboBox1_SelectionChangeCommitted( this, null );
        }

        private void myOkBtn_Click( object sender, EventArgs e )
        {
            Close();
        }

        private void comboBox1_SelectionChangeCommitted( object sender, EventArgs e )
        {
            myBrowser.Navigate( ( (Report)myReports.SelectedItem ).Generator() );
        }
    }
}
