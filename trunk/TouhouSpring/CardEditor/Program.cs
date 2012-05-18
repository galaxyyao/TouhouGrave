using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace TouhouSpring
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
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }

        private static TouhouSpring.PathUtils s_pathUtils = new TouhouSpring.PathUtils(
            Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
            Properties.Settings.Default.ContentRootDirectory));

        public static TouhouSpring.PathUtils PathUtils
        {
            get { return s_pathUtils; }
        }
    }
}
