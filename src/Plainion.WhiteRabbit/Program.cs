using System;
using System.Windows.Forms;
using Plainion.WhiteRabbit.Presentation;

namespace Plainion.WhiteRabbit
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault( false );

            Controller ctrl = new Controller( typeof( MainUI ) );

            Application.Run( ctrl.MainView as Form );
        }
    }
}