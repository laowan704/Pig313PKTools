using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Pig313PKFaceToolPlus
{
    public partial class LoadForm : Form
    {
        public LoadForm()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void LoadForm_Shown(object sender, EventArgs e)
        {
            var t = new Thread(new ParameterizedThreadStart(this.load));
            t.Start(null);
        }

        private void load(object obj)
        {
            for (int i = 1; i <= 8; i++)
            {
                this.label1.Text = "读取资源 (" + i + ") ......";
                ThumbHelper.GetThumbPageImage(i);
                Thread.Sleep(25);
                this.label1.Text = "读取资源 (" + i + ") 完成";
            }
            this.label1.Text = "解析资源 ......";
            var sw = new Stopwatch();
            ThumbHelper.LoadAllThumbs(
                new Action<int>((i) => {
                    this.label1.Text = "解析资源 ("+ i +") ......";
                    sw.Start();
                }),
                new Action<ThumbPage>((page) => {
                    sw.Stop();
                    TimeSpan ts = sw.Elapsed;
                    this.label1.Text = "解析资源 ("+ page.Pageno +") 完成. 用时:"+ Convert.ToInt32(ts.TotalMilliseconds) +"毫秒";
                    sw.Reset();
                    Thread.Sleep(100);
                }));

            //var pages = ThumbHelper.ThumbPages;
            this.Close();
        }
    }
}
