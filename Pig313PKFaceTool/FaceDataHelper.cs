using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Pig313PKFaceTool
{
    static class FaceDataHelper
    {
        const string datafilename = "numberdata.csv";
        const string mydatafilename = "my.dat";
        static string appDir = null;
        static DataTable rawData;

        static DataTable RawData
        {
            get
            {
                if (rawData == null)
                {
                    //rawData = CSVFileHelper.OpenCSV(filepath);
                    Encoding encoding;
                    //using (var stream = ResourceControl.GetResourceStream(datafilename))
                    //{
                        
                    //}
                    using (var stream = ResourceControl.GetResourceStream(datafilename))
                    {
                        encoding = CSVFileHelper.GetStreamEncodeType(stream);
                        rawData = CSVFileHelper.OpenCSV(stream, encoding, false);
                    }
                }
                return rawData;
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
                        for (int i = 0; i < RawData.Rows.Count; i++)
                        {
                            var row = RawData.Rows[i];
                            string genderstr = row[2].ToString();
                            string f81 = row[3].ToString();
                            string f82 = row[4].ToString();
                            string f83 = row[5].ToString();
                            string f13 = row[6].ToString();
                            string f18 = row[7].ToString();
                            string f20 = row[8].ToString();
                            string numstr = f20
                                .Replace("00000020_", string.Empty)
                                .Replace(".bmp", string.Empty);
                            int num = 0;
                            if (int.TryParse(numstr, out num))
                            {
                                var fdata = new FaceData(num, f81, f82, f83, f13, f18, f20, genderstr.Trim() == "男");
                                data[num] = fdata;
                            }

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

    static class ThumbControl
    {
        public const int BLOCK_WIDTH = 100;
        public const int BLOCK_HEIGHT = 100;
        static List<Bitmap> orgthumbs6 = null;
        static List<Bitmap> orgthumbs7 = null;
        static List<Bitmap> orgthumbs8 = null;
        private const string thumb6filename = "6.bmp";
        private const string thumb7filename = "7.bmp";
        private const string thumb8filename = "8.bmp";
        static Bitmap thumb6, thumb7, thumb8;

        public static Bitmap ThumbSix
        {
            get {
                if (thumb6 == null)
                {
                    var img6 = Bitmap.FromStream(ResourceControl.GetResourceStream(thumb6filename));////Bitmap.FromFile(thumb6filename);
                    thumb6 = (Bitmap)img6;
                }
                return thumb6;
            }
        }

        public static Bitmap ThumbSeven
        {
            get
            {
                if (thumb7 == null)
                {
                    var img7 = Bitmap.FromStream(ResourceControl.GetResourceStream(thumb7filename));
                    thumb7 = (Bitmap)img7;
                }
                return thumb7;
            }
        }

        public static Bitmap ThumbEight
        {
            get
            {
                if (thumb8 == null)
                {
                    var img8 = Bitmap.FromStream(ResourceControl.GetResourceStream(thumb8filename));
                    thumb8 = (Bitmap)img8;
                }
                return thumb8;
            }
        }

        public static Bitmap[] ImgsSix
        {
            get
            {
                if (orgthumbs6 == null)
                {
                    orgthumbs6 = Img2Thumb(ThumbSix, 6);
                }
                return orgthumbs6.ToArray();
            }
        }

        public static Bitmap[] ImgsSeven
        {
            get
            {
                if (orgthumbs7 == null)
                {
                    orgthumbs7 = Img2Thumb(ThumbSeven,7);
                }
                return orgthumbs7.ToArray();
            }
        }

        public static Bitmap[] ImgsEight
        {
            get
            {
                if (orgthumbs8 == null)
                {
                    orgthumbs8 = Img2Thumb(ThumbEight, 8);
                }
                return orgthumbs8.ToArray();
            }
        }

        private static List<Bitmap> Img2Thumb(Image img,int page)
        {
            int w = img.Size.Width;
            int h = img.Size.Height;
            int tilexcount = PageColSize(page);//= w / blockwidth;
            int tileycount = PageRowSize(page);//h / blockheight;

            var rs = new List<Bitmap>();
            for (int y = 0; y < tileycount; y++)
            {
                int top = y * BLOCK_HEIGHT;
                for (int x = 0; x < tilexcount; x++)
                {
                    int left = x * BLOCK_WIDTH;
                    var tmpbitmap = new Bitmap(BLOCK_WIDTH, BLOCK_HEIGHT);
                    var tmpg = Graphics.FromImage(tmpbitmap);
                    tmpg.DrawImage(
                        img,
                        new Rectangle(0, 0, BLOCK_WIDTH, BLOCK_HEIGHT),
                        new Rectangle(left, top, BLOCK_WIDTH, BLOCK_HEIGHT),
                        GraphicsUnit.Pixel);
                    tmpg.Dispose();
                    rs.Add(tmpbitmap);
                }
            }
            return rs;
        }

        public static List<Bitmap> Img2Thumb(string filepath)
        {
            var rs = new List<Bitmap>();
            var fimg = Bitmap.FromFile(filepath);
            int w = fimg.Size.Width;
            int h = fimg.Size.Height;
            int tilexcount = w / BLOCK_WIDTH;
            int tileycount = h / BLOCK_HEIGHT;

            for(int y = 0; y < tileycount; y++)
            {
                int top = y * BLOCK_HEIGHT;
                for(int x = 0; x < tilexcount; x++)
                {
                    int left = x * BLOCK_WIDTH;
                    var tmpbitmap = new Bitmap(BLOCK_WIDTH, BLOCK_HEIGHT);
                    var tmpg = Graphics.FromImage(tmpbitmap);
                    tmpg.DrawImage(
                        fimg,
                        new Rectangle(0, 0, BLOCK_WIDTH, BLOCK_HEIGHT),
                        new Rectangle(left, top, BLOCK_WIDTH, BLOCK_HEIGHT),
                        GraphicsUnit.Pixel);
                    tmpg.Dispose();
                    rs.Add(tmpbitmap); 
                }
            }
            return rs;
        }

        private static int PageColSize(int page)
        {
            if (page == 8)
                return 10;
            return 20;
        }

        private static int PageRowSize(int page)
        {
            return 10;
        }

        public static Bitmap GetThumb(int page, int x, int y)
        {
            Bitmap[] bmps = null;
            switch (page)
            {
                case 6:
                    bmps = ImgsSix;
                    break;
                case 7:
                    bmps = ImgsSeven;
                    break;
                case 8:
                    bmps = ImgsEight;
                    break;
            }
            var n = PageColSize(page) * y + x;
            return bmps[n];
        }
    }

    static class OutputHelper
    {
        public static void Output(string outputDir)
        {
            var fdatas = FaceDataHelper.GetFaceDatas();
            var outputThumbFaceDatas = new List<FaceData>();
            foreach (var fdata in fdatas)
            {
                if (fdata.IsCustomed)
                {
                    CopyFile(fdata.Path13, fdata.Filename13, outputDir);
                    CopyFile(fdata.Path18, fdata.Filename18, outputDir);
                    CopyFile(fdata.Path20, fdata.Filename20, outputDir);
                    CopyFile(fdata.Path81, fdata.Filename81, outputDir);
                    CopyFile(fdata.Path82, fdata.Filename82, outputDir);
                    CopyFile(fdata.Path83, fdata.Filename83, outputDir);
                    if (string.IsNullOrEmpty(fdata.Path22) == false)
                    {
                        outputThumbFaceDatas.Add(fdata);
                    }
                }
            }
            if (outputThumbFaceDatas.Count > 0)
            {

                var img226 = new Bitmap(ThumbControl.ThumbSix);// = ThumbControl.ThumbSix;
                var img227 = new Bitmap(ThumbControl.ThumbSeven);// = ThumbControl.ThumbSeven;
                var img228 = new Bitmap(ThumbControl.ThumbEight);// = ThumbControl.ThumbEight;
                bool save6 = false, save7 = false, save8 = false;
                foreach(var fdata in outputThumbFaceDatas)
                {
                    Graphics g = null;
                    switch(fdata.PageInThumb)
                    {
                        case 6:
                            g = Graphics.FromImage(img226);
                            save6 = true;
                            break;
                        case 7:
                            g = Graphics.FromImage(img227);
                            save7 = true;
                            break;
                        case 8:
                            g = Graphics.FromImage(img228);
                            save8 = true;
                            break;
                    }
                    var simg = Bitmap.FromFile(fdata.Path22);
                    g.DrawImage(simg,
                        new Rectangle(
                            fdata.XPosInThumb * ThumbControl.BLOCK_WIDTH,
                            fdata.YPosInThumb * ThumbControl.BLOCK_HEIGHT,
                            ThumbControl.BLOCK_WIDTH,
                            ThumbControl.BLOCK_HEIGHT),
                        new Rectangle(0, 0, ThumbControl.BLOCK_WIDTH, ThumbControl.BLOCK_HEIGHT),
                        
                        GraphicsUnit.Pixel);
                    g.Dispose();
                }
                if (save6)
                    SaveImage(img226, "00000022_00006.bmp", outputDir);
                if (save7)
                    SaveImage(img227, "00000022_00007.bmp", outputDir);
                if (save8)
                    SaveImage(img228, "00000022_00008.bmp", outputDir);
            }
            FaceDataHelper.SaveMyData();
        }

        static void CopyFile(string srcPath, string tgtFileName, string tgtDirPath)
        {
            if (string.IsNullOrEmpty(srcPath))
                return;
            try
            {
                if (File.Exists(srcPath))
                {
                    var tgtPath = Path.Combine(tgtDirPath, tgtFileName);
                    if (File.Exists(tgtPath))
                    {
                        File.Delete(tgtPath);
                    }
                    var tmpFileName = Guid.NewGuid().ToString();
                    var tmpPath = Path.Combine(tgtDirPath, tmpFileName);
                    File.Copy(srcPath, tmpPath);
                    File.Move(tmpPath, tgtPath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("OutputHelper.CopyFile异常.", ex);
            }
        }

        public static void SaveImage(Image img, string tgtFileName, string tgtDirPath)
        {
            try
            {
                var tgtPath = Path.Combine(tgtDirPath, tgtFileName);
                if (File.Exists(tgtPath))
                {
                    File.Delete(tgtPath);
                }
                var tmpFileName = Guid.NewGuid().ToString();
                var tmpPath = Path.Combine(tgtDirPath, tmpFileName);
                img.Save(tmpPath, System.Drawing.Imaging.ImageFormat.Bmp);
                File.Move(tmpPath, tgtPath);
            }
            catch (Exception ex)
            {
                throw new Exception("OutputHelper.SaveImage异常.", ex);
            }
        }
    }
}