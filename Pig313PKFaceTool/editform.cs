using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Pig313PKFaceTool
{
    public partial class EditForm : Form
    {
        OpenFileDialog dlg;
        FaceData fdata;
        string f81, f82, f83, f13, f18, f20, f22;
        bool moded = false;
        public EditForm()
        {
            InitializeComponent();
            this.btn13.AllowDrop
                = this.btn18.AllowDrop
                = this.btn20.AllowDrop
                = this.btn22.AllowDrop
                = this.btn81.AllowDrop
                = this.btn82.AllowDrop
                = this.btn83.AllowDrop
                = true;
            dlg = new OpenFileDialog();
            dlg.Filter = "Bitmap File|*.BMP";
            dlg.CheckFileExists = true;
            dlg.RestoreDirectory = false;
            dlg.Multiselect = false;
        }

        public void SetFaceData(FaceData fdata, Image imgsrc)
        {
            this.picSrc.Image = imgsrc;
            this.lblSrc.Text = fdata.Number.ToString();
            this.fdata = fdata;
            this.f13 = this.fdata.Path13;
            this.f18 = this.fdata.Path18;
            this.f20 = this.fdata.Path20;
            this.f22 = this.fdata.Path22;
            this.f81 = this.fdata.Path81;
            this.f82 = this.fdata.Path82;
            this.f83 = this.fdata.Path83;
            this.SetButtonImage(this.btn13, this.f13);
            this.SetButtonImage(this.btn18, this.f18);
            this.SetButtonImage(this.btn20, this.f20);
            this.SetButtonImage(this.btn22, this.f22);
            this.SetButtonImage(this.btn81, this.f81);
            this.SetButtonImage(this.btn82, this.f82);
            this.SetButtonImage(this.btn83, this.f83);
            this.moded = false;
        }

        public FaceData GetFaceData()
        {
            if (this.TryUpdateFaceData())
            {

            }
            return null;
        }

        private bool TryUpdateFaceData()
        {
            if (moded)
            {
                this.fdata.Path13 = this.f13;
                this.fdata.Path18 = this.f18;
                this.fdata.Path20 = this.f20;
                this.fdata.Path22 = this.f22;
                this.fdata.Path81 = this.f81;
                this.fdata.Path82 = this.f82;
                this.fdata.Path83 = this.f83;
            }
            return moded;
        }

        private void btnClick(int btnnum)
        {
            var btn = this.GetButtonByNum(btnnum);
            {
                dlg.Title = "打开文件 - " + btnnum;
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    var filepath = dlg.FileName;
                    this.SetImagePath(btnnum, filepath, btn);
                }
            }
        }

        private void SetButtonImage(Button btn, string filepath)
        {
            int num = 0;
            int.TryParse(btn.Tag.ToString(), out num);
            var lbl = this.GetLabelByNum(num);
            if (string.IsNullOrEmpty(filepath))
            {
                btn.Image = null;
                lbl.Text = "0x0";
            }
            else
            {
                var img = Bitmap.FromFile(filepath);
                lbl.Text = img.Width + "x" + img.Height;
                btn.Image = img.GetThumbnailImage(btn.Width, btn.Height, dummyCallback, IntPtr.Zero);
            }
        }

        static bool ThumbnailCallback()
        {
            return false;
        }
        Image.GetThumbnailImageAbort dummyCallback = new Image.GetThumbnailImageAbort(ThumbnailCallback);

        private void picDragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                String[] files = e.Data.GetData(DataFormats.FileDrop, false) as String[];
                if (files.Length > 1)
                {
                    return;
                }
                string filepath = files[0];
                var ext = Path.GetExtension(filepath);
                if (ext != ".bmp")
                    return;
                var btn = sender as Button;
                int num = 0;
                int.TryParse(btn.Tag.ToString(), out num);
                this.SetImagePath(num, filepath, btn);
            }
        }

        private void picMouseEnter(object sender, EventArgs e)
        {
            var btn = sender as Button;
            int num = 0;
            int.TryParse(btn.Tag.ToString(), out num);
            string str = null;
            switch (num)
            {
                case 13:
                    str = this.f13;
                    break;
                case 18:
                    str = this.f18;
                    break;
                case 20:
                    str = this.f20;
                    break;
                case 22:
                    str = this.f22;
                    break;
                case 81:
                    str = this.f81;
                    break;
                case 82:
                    str = this.f82;
                    break;
                case 83:
                    str = this.f83;
                    break;
                default:
                    return;
            }
            if (string.IsNullOrEmpty(str))
                str = num.ToString();
            else
                str = num.ToString() + System.Environment.NewLine + str;
            this.toolTip1.Show(str, sender as IWin32Window);

        }

        private void picMouseLeave(object sender, EventArgs e)
        {
            this.toolTip1.Hide(sender as IWin32Window);
        }

        private void SetImagePath(int num, string filepath, Button btn = null)
        {
            if (btn == null)
            {
                btn = this.GetButtonByNum(num);
            }
            switch (num)
            {
                case 81:
                    this.f81 = filepath;
                    break;
                case 82:
                    this.f82 = filepath;
                    break;
                case 83:
                    this.f83 = filepath;
                    break;
                case 13:
                    this.f13 = filepath;
                    break;
                case 18:
                    this.f18 = filepath;
                    break;
                case 20:
                    this.f20 = filepath;
                    break;
                case 22:
                    this.f22 = filepath;
                    break;
                default:
                    return;
            }
            SetButtonImage(btn, filepath);
            this.moded = true;
        }

        private Button GetButtonByNum(int num)
        {
            switch (num)
            {
                case 13:
                    return btn13;
                case 18:
                    return btn18;
                case 20:
                    return btn20;
                case 22:
                    return btn22;
                case 81:
                    return btn81;
                case 82:
                    return btn82;
                case 83:
                    return btn83;
                default:
                    return null;
            }
        }

        private Label GetLabelByNum(int num)
        {
            switch (num)
            {
                case 13:
                    return lbl13;
                case 18:
                    return lbl18;
                case 20:
                    return lbl20;
                case 22:
                    return lbl22;
                case 81:
                    return lbl81;
                case 82:
                    return lbl82;
                case 83:
                    return lbl83;
                default:
                    return null;
            }
        }

        private void picDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                String[] files = e.Data.GetData(DataFormats.FileDrop, false) as String[];
                if (files.Length == 1)
                {
                    string file = files[0];
                    var ext = Path.GetExtension(file);
                    if (ext == ".bmp")
                    {
                        e.Effect = DragDropEffects.Copy;
                        return;
                    }
                }
            }
            e.Effect = DragDropEffects.None;
        }

        #region button click
        private void btn81_Click(object sender, EventArgs e)
        {
            this.btnClick(81);
        }

        private void btn82_Click(object sender, EventArgs e)
        {
            this.btnClick(82);
        }

        private void btn83_Click(object sender, EventArgs e)
        {
            this.btnClick(83);
        }

        private void btn13_Click(object sender, EventArgs e)
        {
            this.btnClick(13);
        }

        private void btn18_Click(object sender, EventArgs e)
        {
            this.btnClick(18);
        }

        private void btn20_Click(object sender, EventArgs e)
        {
            this.btnClick(20);
        }

        private void btn22_Click(object sender, EventArgs e)
        {
            this.btnClick(22);
        }
        #endregion

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            if (string.IsNullOrEmpty(this.f81) == false)
                this.fdata.Path81 = this.f81;
            if (string.IsNullOrEmpty(this.f82) == false)
                this.fdata.Path82 = this.f82;
            if (string.IsNullOrEmpty(this.f83) == false)
                this.fdata.Path83 = this.f83;
            if (string.IsNullOrEmpty(this.f13) == false)
                this.fdata.Path13 = this.f13;
            if (string.IsNullOrEmpty(this.f18) == false)
                this.fdata.Path18 = this.f18;
            if (string.IsNullOrEmpty(this.f20) == false)
                this.fdata.Path20 = this.f20;
            if (string.IsNullOrEmpty(this.f22) == false)
                this.fdata.Path22 = this.f22;
        }
    }
}
