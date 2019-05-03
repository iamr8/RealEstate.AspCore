using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace RealEstate.Runner
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            using (var consoleWriter = new ConsoleWriter())
            {
                consoleWriter.WriteEvent += consoleWriter_WriteEvent;
                consoleWriter.WriteLineEvent += consoleWriter_WriteLineEvent;

                Console.SetOut(consoleWriter);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.ApplicationExit += ApplicationOnApplicationExit;
                Application.Run(new Form1());
            }
        }

        public static void KillDotNet()
        {
            foreach (var process in Process.GetProcesses())
            {
                if (!process.ProcessName.Contains("dotnet"))
                    continue;

                process.Kill();
            }
        }

        private static void ApplicationOnApplicationExit(object sender, EventArgs e)
        {
            KillDotNet();
        }

        private static void consoleWriter_WriteLineEvent(object sender, ConsoleWriterEventArgs e)
        {
            MessageBox.Show(e.Value, "WriteLine");
        }

        private static void consoleWriter_WriteEvent(object sender, ConsoleWriterEventArgs e)
        {
            MessageBox.Show(e.Value, "Write");
        }
    }
}