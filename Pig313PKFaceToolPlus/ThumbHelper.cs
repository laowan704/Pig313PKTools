using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Pig313PKFaceToolPlus
{
    static class ThumbHelper
    {
        public const int BLOCK_WIDTH = 100;
        public const int BLOCK_HEIGHT = 100;
        static ThumbPage[] thumbPages = null;

        static Dictionary<int, Image> imgcache = null;
        public static Image GetThumbPageImage(int pageno)
        {
            if (imgcache == null)
                imgcache = new Dictionary<int, Image>();
            if (imgcache.Keys.Contains(pageno) == false)
            {
                imgcache[pageno] = Bitmap.FromStream(ResourceControl.GetResourceStream(ThumbHelper.GetThumbPageFileName(pageno)));
                //imgcache[pageno] = Bitmap.FromFile(ThumbHelper.GetThumbPageFileName(pageno));
            }
            return imgcache[pageno];
        }

        public static ThumbPage[] ThumbPages
        {
            get
            {
                if (thumbPages == null)
                    LoadAllThumbs();
                return thumbPages;
            }
        }

        public static ThumbPage GetThumbPage(int pageno)
        {
            return ThumbPages[pageno - 1];
        }

        private static List<Bitmap> Img2Thumb(Image img, int page)
        {
            int w = img.Size.Width;
            int h = img.Size.Height;
            int tilexcount = w / BLOCK_WIDTH;
            int tileycount = h / BLOCK_HEIGHT;

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
            int x, y;
            return Img2Thumb(filepath, out x, out y);
        }

        public static List<Bitmap> Img2Thumb(string filepath, out int tilexcount, out int tileycount)
        {
            var rs = new List<Bitmap>();
            var fimg = Bitmap.FromFile(filepath);
            int w = fimg.Size.Width;
            int h = fimg.Size.Height;
             tilexcount = w / BLOCK_WIDTH;
             tileycount = h / BLOCK_HEIGHT;

            for (int y = 0; y < tileycount; y++)
            {
                int top = y * BLOCK_HEIGHT;
                for (int x = 0; x < tilexcount; x++)
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

        public static string GetThumbPageFileName(int pageno)
        {
            if (pageno > 0 && pageno <= 8)
            {
                return (pageno + 0) + ".bmp";
            }
            return null;
        }

        public static void LoadAllThumbs(Action<int> beforeLoadPage = null, Action<ThumbPage> afterLoadPage = null)
        {
            int lastpageLastNumber = 0;
            var rs = new List<ThumbPage>();
            for (int i = 1; i <= 8; i++)
            {
                beforeLoadPage?.Invoke(i);
                var pageobj = LoadThumbPage(i, lastpageLastNumber);
                afterLoadPage?.Invoke(pageobj);
                rs.Add(pageobj);
                lastpageLastNumber += pageobj.ThumbCount;
            }
            thumbPages = rs.ToArray();
            return;
        }

        static ThumbPage LoadThumbPage(int pageno, int thumbNumberStart)
        {
            var fimg = GetThumbPageImage(pageno); //Bitmap.FromFile(filename);
            int w = fimg.Size.Width;
            int h = fimg.Size.Height;
            int tilexcount = w / BLOCK_WIDTH;
            int tileycount = h / BLOCK_HEIGHT;
            
            var thumbs = new List<ThumbImage>();
            int f = 0;
            for (int y = 0; y < tileycount; y++)
            {
                int top = y * BLOCK_HEIGHT;
                for (int x = 0; x < tilexcount; x++)
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
                    var thumb = new ThumbImage(pageno, x, y, f, tmpbitmap);
                    thumbs.Add(thumb);
                    f++;
                }
            }
            
            return new ThumbPage(pageno, thumbs, thumbNumberStart + thumbs.Count, tilexcount, tileycount);
        }

        

        public static Bitmap GetThumb(int pageno, int x, int y)
        {
            var page = ThumbPages[pageno - 1];
            return page.GetThumb(x, y).Image;
        }

        public static Bitmap GetThumb(int thumbNumber)
        {
            return null;
        }
    }

    class ThumbImage
    {
        public ThumbImage(int page, int x, int y, int indexInPage, Bitmap img)
        {
            this.page = page;
            this.x = x;
            this.y = y;
            this.img = img;
            this.indexInPage = indexInPage;
        }
        Bitmap img;
        int page;
        int x, y, indexInPage;

        public Bitmap Image
        {
            get
            {
                return img;
            }

        }

        public int Page
        {
            get
            {
                return page;
            }
        }

        public int X
        {
            get
            {
                return x;
            }

        }

        public int Y
        {
            get
            {
                return y;
            }

        }

        public int IndexInPage
        {
            get
            {
                return indexInPage;
            }
        }
    }

    class ThumbPage
    {
        int pageno = 0;
        int thumbStartNumber = 0;
        int colSize, rowSize;
        List<ThumbImage> thumbs = null;

        public ThumbPage(int page, List<ThumbImage> thumbs, int thumbStartNumber, int colsize, int rowsize)
        {
            this.pageno = page;
            this.thumbs = thumbs;
            this.thumbStartNumber = thumbStartNumber;
            this.colSize = colsize;
            this.rowSize = rowsize;
        }

        public ThumbImage[] Thumbs { get { return this.thumbs.ToArray(); } }

        public int Pageno
        {
            get
            {
                return pageno;
            }
        }

        public int ThumbStartNumber
        {
            get
            {
                return thumbStartNumber;
            }
        }

        public int ThumbCount { get { return this.thumbs.Count; } }

        public int ColSize
        {
            get
            {
                return colSize;
            }
        }

        public int RowSize
        {
            get
            {
                return rowSize;
            }
        }

        public ThumbImage GetThumb(int x, int y)
        {
            int idx = y * colSize + x;
            return thumbs[idx];
        }

        public ThumbImage GetThumb(int thumbIndex) { return this.thumbs[thumbIndex]; }
    }
}
