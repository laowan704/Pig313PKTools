using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Pig313PKFaceToolPlus
{

    [Serializable]
    public class FaceData
    {
        string path81, path82, path83, path13, path20, path22, path18;
        string filename81, filename82, filename83, filename13, filename20, filename18;
        bool moded = false;
        int number;
        public FaceData(int number, string filename81, string filename82, string filename83, 
            string filename13, string filename18, string filename20, int pageInThumb, int xInThumb, int yInThumb)
        {
            this.number = number;
            this.filename81 = filename81;
            this.filename82 = filename82;
            this.filename83 = filename83;
            this.filename13 = filename13;
            this.filename20 = filename20;
            this.filename18 = filename18;
            this.pageInThumb = pageInThumb;
            this.xInThumb = xInThumb;
            this.yInThumb = yInThumb;
        }

        public bool IsCustomed
        {
            get
            {
                return !string.IsNullOrEmpty(Path13)
                    || !string.IsNullOrEmpty(Path18)
                    ||!string.IsNullOrEmpty(Path20)
                    ||!string.IsNullOrEmpty(Path22)
                    ||!string.IsNullOrEmpty(Path81)
                    ||!string.IsNullOrEmpty(Path82)
                    || !string.IsNullOrEmpty(Path83);
            }
        }

        int xInThumb, yInThumb, pageInThumb;

        public int XPosInThumb
        {
            get
            {
                return xInThumb;
            }
        }

        public int YPosInThumb
        {
            get
            {
                return yInThumb;
            }
        }

        public int PageInThumb
        {
            get
            {
                return pageInThumb;
            }
        }

        public string Path81
        {
            get
            {
                if (string.IsNullOrEmpty(path81) == false)
                {
                    if (File.Exists(path81) == false)
                        path81 = null;
                }
                return path81;
            }

            set
            {
                path81 = value; this.moded = true;
            }
        }

        public string Path82
        {
            get
            {
                if (string.IsNullOrEmpty(path82) == false)
                {
                    if (File.Exists(path82) == false)
                        path82 = null;
                }
                return path82;
            }

            set
            {
                path82 = value; this.moded = true;
            }
        }

        public string Path83
        {
            get
            {
                if (string.IsNullOrEmpty(path83) == false)
                {
                    if (File.Exists(path83) == false)
                        path83 = null;
                }
                return path83;
            }

            set
            {
                path83 = value; this.moded = true;
            }
        }

        public string Path13
        {
            get
            {
                if (string.IsNullOrEmpty(path13) == false)
                {
                    if (File.Exists(path13) == false)
                        path13 = null;
                }
                return path13;
            }

            set
            {
                path13 = value; this.moded = true;
            }
        }

        public string Path20
        {
            get
            {
                if (string.IsNullOrEmpty(path20) == false)
                {
                    if (File.Exists(path20) == false)
                        path20 = null;
                }
                return path20;
            }

            set
            {
                path20 = value; this.moded = true;
            }
        }

        public string Path22
        {
            get
            {
                if (string.IsNullOrEmpty(path22) == false)
                {
                    if (File.Exists(path22) == false)
                        path22 = null;
                }
                return path22;
            }

            set
            {
                path22 = value; this.moded = true;
            }
        }

        public string Path18
        {
            get
            {
                if (string.IsNullOrEmpty(path18) == false)
                {
                    if (File.Exists(path18) == false)
                        path18 = null;
                }
                return path18;
            }

            set
            {
                path18 = value; this.moded = true;
            }
        }

        public int Number
        {
            get
            {
                return number;
            }
        }

        public bool Moded
        {
            get
            {
                return moded;
            }
        }

        public string Filename81
        {
            get
            {
                return filename81;
            }
        }

        public string Filename82
        {
            get
            {
                return filename82;
            }
        }

        public string Filename83
        {
            get
            {
                return filename83;
            }
        }

        public string Filename13
        {
            get
            {
                return filename13;
            }
        }

        public string Filename20
        {
            get
            {
                return filename20;
            }
        }

        public string Filename18
        {
            get
            {
                return filename18;
            }
        }
    }
}
