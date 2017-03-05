using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Pig313PKFaceTool
{
    class ResourceControl
    {
        public static Stream GetResourceStream(string filename)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream("Pig313PKFaceTool." + filename);
        }
    }
}
