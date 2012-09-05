using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Mogre.Demo.MogreForm
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static unsafe void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MogreForm());
        }
    }
}