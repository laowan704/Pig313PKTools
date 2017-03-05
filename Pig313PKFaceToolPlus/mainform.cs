using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Pig313PKFaceToolPlus
{
    public partial class MainForm : Form
    {
        const int buttonsize = 50;
        EditForm dlg;
        FlowLayoutPanel[] flowPanels = null;
        public MainForm()
        {
            InitializeComponent();
            this.flowPanels = new FlowLayoutPanel[]{
                this.flowLayoutPanel1, this.flowLayoutPanel2,
                this.flowLayoutPanel3, this.flowLayoutPanel4,
                this.flowLayoutPanel5, this.flowLayoutPanel6,
                this.flowLayoutPanel7, this.flowLayoutPanel8
            };
            var datas = FaceDataHelper.GetFaceDatas();
            foreach (var data in datas)
            {
                var ctrl = new Button();
                this.ButtonBindFaceData(ctrl, data);
                ctrl.Width = buttonsize;
                ctrl.Height = buttonsize;
                ctrl.Tag = data;
                ctrl.Click += Ctrl_Click;
                ctrl.MouseUp += Ctrl_MouseUp;
                //ctrl.ForeColor = Color.GreenYellow;
                ctrl.Font = new Font(new FontFamily("宋体"), 7, FontStyle.Bold);
                ctrl.TextAlign = ContentAlignment.BottomCenter;
                var pnl = this.flowPanels[data.PageInThumb - 1];
                ctrl.Margin = new Padding(0);
                pnl.Controls.Add(ctrl);
                if (data.Number % 10 == 0)
                {
                    var page = ThumbHelper.GetThumbPage(data.PageInThumb);
                    if (data.XPosInThumb + 1 == page.ColSize)
                        pnl.SetFlowBreak(ctrl, true);
                }
                ctrl.Show();
            }
            dlg = new Pig313PKFaceToolPlus.EditForm();
        }



        private void ButtonBindFaceData(Button btn, FaceData fdata)
        {
            Image img = null;
            if (string.IsNullOrEmpty(fdata.Path22))
            {
                img = ThumbHelper.GetThumb(fdata.PageInThumb, fdata.XPosInThumb, fdata.YPosInThumb);
            }
            else
            {
                img = Bitmap.FromFile(fdata.Path22);
            }
            var bmp = new Bitmap(img, new Size(buttonsize, buttonsize));
            btn.Image = bmp;
            btn.Text = fdata.Number.ToString("D5");
            if (fdata.IsCustomed)
                btn.ForeColor = Color.Red;
            else
                btn.ForeColor = Color.GreenYellow;
        }

        private void Ctrl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var btn = ((Button)sender);
                var fdata = btn.Tag as FaceData;
                if (fdata.IsCustomed)
                {
                    if (MessageBox.Show("是否重置", "重置", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        ResetFaceData(fdata);
                        this.ButtonBindFaceData(btn, fdata);
                    }
                }
                return;
            }
        }

        private void Ctrl_Click(object sender, EventArgs e)
        {
            var btn = ((Button)sender);
            var fdata = btn.Tag as FaceData;
            this.dlg.SetFaceData(fdata, btn.Image);
            if (this.dlg.ShowDialog(this) == DialogResult.OK)
            {
                this.ButtonBindFaceData(btn, fdata);
            }
        }

        private void ResetFaceData(FaceData fdata)
        {
            if (string.IsNullOrEmpty(fdata.Path13) == false)
                fdata.Path13 = null;
            if (string.IsNullOrEmpty(fdata.Path18) == false)
                fdata.Path18 = null;
            if (string.IsNullOrEmpty(fdata.Path20) == false)
                fdata.Path20 = null;
            if (string.IsNullOrEmpty(fdata.Path22) == false)
                fdata.Path22 = null;
            if (string.IsNullOrEmpty(fdata.Path81) == false)
                fdata.Path81 = null;
            if (string.IsNullOrEmpty(fdata.Path82) == false)
                fdata.Path82 = null;
            if (string.IsNullOrEmpty(fdata.Path83) == false)
                fdata.Path83 = null;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("是否重置全部修改", "重置", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            foreach (var pnl in this.flowPanels)
            {
                foreach (Control ctrl in pnl.Controls)
                {
                    if (ctrl is Button && ctrl.Tag is FaceData)
                    {
                        var btn = ctrl as Button;
                        var fdata = btn.Tag as FaceData;
                        this.ResetFaceData(fdata);
                        this.ButtonBindFaceData(btn, fdata);
                    }
                }
            }
            FaceDataHelper.SaveMyData();
        }

        private void btnDo_Click(object sender, EventArgs e)
        {
            bool needsave = false;
            foreach (var fdata in FaceDataHelper.GetFaceDatas())
            {
                if (fdata.IsCustomed)
                {
                    needsave = true;
                    break;
                }
            }
            if (needsave == false)
                return;
            using (var fdlg = new FolderBrowserDialog())
            {
                fdlg.ShowNewFolderButton = true;
                if (fdlg.ShowDialog(this) == DialogResult.OK)
                {
                    var outputdir = fdlg.SelectedPath;
                    OutputHelper.Output(outputdir);
                    var dr = MessageBox.Show("已保存,是否打开文件夹", "已保存", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (dr == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(outputdir);
                    }
                }

            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            FaceDataHelper.SaveMyData();
        }

        private void btnSplit22_Click(object sender, EventArgs e)
        {
            using (var odlg = new OpenFileDialog())
            {
                odlg.Filter = "Bitmap File|*.BMP";
                odlg.CheckFileExists = true;
                odlg.RestoreDirectory = false;
                odlg.Multiselect = false;
                odlg.Title = "请选择待拆分的22图片";
                if (odlg.ShowDialog(this) == DialogResult.OK)
                {
                    using (var fdlg = new FolderBrowserDialog())
                    {
                        fdlg.ShowNewFolderButton = true;
                        if (fdlg.ShowDialog(this) == DialogResult.OK)
                        {
                            var targetDir = fdlg.SelectedPath;
                            var thumbs = ThumbHelper.Img2Thumb(odlg.FileName);
                            var dmyname = DateTime.Now.ToString("yyyyMMddHHmmss") + "_";
                            for (int i = 0; i < thumbs.Count; i++)
                            {
                                OutputHelper.SaveImage(thumbs[i], dmyname + (i + 1).ToString("D3") + ".bmp", targetDir);
                            }
                            var dr = MessageBox.Show("已保存,是否打开文件夹", "已保存", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                            if (dr == DialogResult.Yes)
                            {
                                System.Diagnostics.Process.Start(targetDir);
                            }
                        }
                    }
                }
            }
        }
    }
}
