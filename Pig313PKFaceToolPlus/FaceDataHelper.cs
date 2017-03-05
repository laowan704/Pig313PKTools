using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

namespace Pig313PKFaceToolPlus
{
    static class FaceDataHelper
    {
        const string mydatafilename = "myplus.dat";
        static string appDir = null;
        static DataTable rawData;

        

        static DataTable RawData
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        static Dictionary<int, FaceData> data = null;
        static Dictionary<int, FaceData> Data
        {
            get
            {
                if (data == null)
                {
                    if (LoadMyData() == false)
                    {
                        data = new Dictionary<int, FaceData>();

                        var thumbPages = ThumbHelper.ThumbPages;
                        int lastpageLastNumber = 0;
                        for (int i = 0; i < thumbPages.Length; i++)
                        {
                            var thumbPage = thumbPages[i];
                            for (int j = 0; j < thumbPage.ThumbCount; j++)
                            {
                                var no = j + 1 + lastpageLastNumber;
                                var thumb = thumbPage.GetThumb(j);
                                var fname81 = "00000008_" + (no * 3 - 2).ToString("D5") + ".bmp";
                                var fname82 = "00000008_" + (no * 3 - 1).ToString("D5") + ".bmp";
                                var fname83 = "00000008_" + (no * 3 - 0).ToString("D5") + ".bmp";
                                var fname13 = "00000013_" + (no).ToString("D5") + ".bmp";
                                var fname18 = "00000018_" + (no).ToString("D5") + ".bmp";
                                var fname20 = "00000020_" + (no).ToString("D5") + ".bmp";
                                var fdata = new FaceData(no, fname81, fname82, fname83, fname13, fname18, fname20, thumbPage.Pageno, thumb.X, thumb.Y);
                                data[no] = fdata;
                            }
                            lastpageLastNumber += thumbPage.ThumbCount;
                        }
                    }
                }
                return data;
            }
        }

        public static string AppDir
        {
            get
            {
                return appDir;
            }

            set
            {
                appDir = value;
            }
        }

        public static FaceData GetFaceData(int num)
        {
            return Data[num];
        }

        public static FaceData[] GetFaceDatas()
        {
            return Data.Values.ToArray();
        }

        public static void SaveMyData()
        {
            var path = Path.Combine(AppDir, mydatafilename);
            using (var fs = new FileStream(path, FileMode.Create))
            {
                var bf = new BinaryFormatter();
                bf.Serialize(fs, GetFaceDatas());
            }
        }

        public static bool LoadMyData()
        {
            bool sec = false;
            try
            {
                var path = Path.Combine(AppDir, mydatafilename);
                using (var fs = new FileStream(path, FileMode.Open))
                {
                    var bf = new BinaryFormatter();
                    object mydataobj = bf.Deserialize(fs);
                    if (data == null)
                        data = new Dictionary<int, FaceData>();
                    else
                        data.Clear();
                    foreach (FaceData md in mydataobj as object[])
                    {
                        data[md.Number] = md;
                    }
                }
                sec = true;
            }
            catch (Exception ex)
            {
            }

            return sec;
        }
    }


    
}