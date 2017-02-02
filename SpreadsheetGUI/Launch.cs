// Developed by Snehashish Mishra, u0946268 on 8th March for
// CS 3500 offered by The University of Utah

using System;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    /// <summary>
    /// Launcher class for the Spreadhseet application
    /// </summary>
    static class Launch
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Get the application context and run one form inside it
            var context = SpreadsheetApplicationContext.GetContext();
            context.RunNew();
            Application.Run(context);
        }
    }
}
