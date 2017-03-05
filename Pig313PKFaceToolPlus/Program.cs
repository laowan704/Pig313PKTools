using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Pig313PKFaceToolPlus
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FaceDataHelper.AppDir = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new LoadForm());
            Application.Run(new MainForm());
        }
    }
}
