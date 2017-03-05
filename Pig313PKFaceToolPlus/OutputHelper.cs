using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Pig313PKFaceToolPlus
{
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
                Dictionary<int, Image> drawImageCache = new Dictionary<int, Image>();
                List<int> pageChanged = new List<int>();
                foreach (var fdata in outputThumbFaceDatas)
                {
                    var pageno = fdata.PageInThumb;
                    if (drawImageCache.Keys.Contains(pageno) == false)
                    {
                        drawImageCache[fdata.PageInThumb] = new Bitmap(ThumbHelper.GetThumbPageImage(fdata.PageInThumb));
                    }
                    var g = Graphics.FromImage(drawImageCache[pageno]);
                    var simg = Bitmap.FromFile(fdata.Path22);
                    g.DrawImage(simg,
                        new Rectangle(
                            fdata.XPosInThumb * ThumbHelper.BLOCK_WIDTH,
                            fdata.YPosInThumb * ThumbHelper.BLOCK_HEIGHT,
                            ThumbHelper.BLOCK_WIDTH,
                            ThumbHelper.BLOCK_HEIGHT),
                        new Rectangle(0, 0, ThumbHelper.BLOCK_WIDTH, ThumbHelper.BLOCK_HEIGHT),
                        GraphicsUnit.Pixel);
                    g.Dispose();
                    if (pageChanged.Contains(pageno) == false)
                    {
                        pageChanged.Add(pageno);
                    }
                }
                foreach (var pageno in pageChanged)
                {
                    SaveImage(drawImageCache[pageno], "00000022_0000" + pageno + ".bmp", outputDir);
                }
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
