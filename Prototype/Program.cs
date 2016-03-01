using System;
using System.Windows.Forms;

namespace EGMPackageBuilder
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            /*Console.WriteLine("Available arguments are:");
            Console.WriteLine("Make sources:");
            Console.WriteLine("\t-s \"Source path\" \"Destination path\" \"Description\"");
            Console.WriteLine("Increase game version");
            Console.WriteLine("-i \"Source path\"");*/
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());
        }
    }
}
