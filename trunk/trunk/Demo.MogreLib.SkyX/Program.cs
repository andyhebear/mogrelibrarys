using System;
using System.Collections.Generic;
using System.Text;
using Mogre.Demo.MogreForm;
using System.Windows.Forms;

namespace Demo.MogreLib.SkyX {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static unsafe void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try {
                //SkyX app = new SkyX();
                //app.Go();
                Application.Run(new MogreForm());
            }
            catch {
                // Check if it's an Ogre Exception
                if (Mogre.OgreException.IsThrown)
                    MessageBox.Show(Mogre.OgreException.LastException.FullDescription);
                else
                    throw;
            }
        }
    }
}
