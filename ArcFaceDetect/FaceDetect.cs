using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ArcFaceDetect
{
    public partial class FaceDetect : Form
    {
        public FaceDetect()
        {
            InitializeComponent();
            Load += FaceDetect_Load;
        }

        #region 屏幕显示切换
        bool isFullScreen = false;
        /// <summary>
        /// 显示切换
        /// </summary>
        private void ChangeScreenModel()
        {
            if (isFullScreen)
            {
                this.FormBorderStyle = FormBorderStyle.FixedDialog;     //设置窗体为无边框样式
                this.WindowState = FormWindowState.Normal;    //最大化窗体 
            }
            else
            {
                this.FormBorderStyle = FormBorderStyle.None;     //设置窗体为无边框样式
                this.WindowState = FormWindowState.Maximized;    //最大化窗体 
            }
            isFullScreen = !isFullScreen;
        }

        private void FaceDetect_Load(object sender, EventArgs e)
        {
            ChangeScreenModel();
        }

        private void FaceDetect_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F11)
            {
                ChangeScreenModel();
            }
        }
        #endregion
    }
}
