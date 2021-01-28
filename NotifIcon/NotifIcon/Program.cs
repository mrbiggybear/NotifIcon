using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using bb_SysTray;

namespace SysTrayControls
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Show the system tray icon.
            using (SysTrayIcon sti = new SysTrayIcon())
            {
                sti.UpdateDisplay();

                // Make sure the application runs!
                Application.Run();
                Console.WriteLine("Done!");
            }
        }
    }
}