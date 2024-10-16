using System;
using System.Windows.Forms;

namespace RepoUtl
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

#if DEBUG
            UnitTests.Execute();
#endif

            try
            {
                Application.Run(new Form1());
            }
            catch (Exception ee)
            {
                var x = ee.Message;
            }
        }
    }
}