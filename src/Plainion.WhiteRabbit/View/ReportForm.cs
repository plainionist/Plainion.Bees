using System;
using System.Windows.Forms;
using Plainion.WhiteRabbit.Presentation;

namespace Plainion.WhiteRabbit.View
{
    public enum ReportScope
    {
        Day,
        Week,
        Month
    }

    public partial class ReportForm : Form
    {
        public ReportForm(Controller controller, ReportScope scope)
        {
            InitializeComponent();

            if (scope == ReportScope.Day)
            {
                myBrowser.Navigate(controller.GenerateDayReport(controller.CurrentDay));
            }
            else if (scope == ReportScope.Week)
            {
                myBrowser.Navigate(controller.GenerateWeekReport(controller.CurrentDay));
            }
            else if (scope == ReportScope.Month)
            {
                myBrowser.Navigate(controller.GenerateMonthReport(controller.CurrentDay));
            }
        }

        private void myOkBtn_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
