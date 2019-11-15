using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace FaceDetect
{
    /// <summary>
    /// 打开摄像头并动态显示图片到图片框的业务逻辑类
    /// </summary>
    public class OpenCamera
    {
        //更新图片线程用的委托回调变量
        internal delegate void UpdateImageDelegate();
        internal UpdateImageDelegate UpdateImageThreadCallBack_OC;

        /// <summary>
        /// 用于获取<see cref="OpenCameraWithOpencvSharp.MainFrm.pictureBox1"/>
        /// </summary>
        internal static PictureBox PictureBox { set; get; }
        /// <summary>
        /// 用于获取或存储摄像头设备变量
        /// </summary>
        internal static VideoCapture VideoCapture_OS;
        public OpenCamera()
        {

        }
        /// <summary>
        /// 构造方法，用于传递<see cref="OpenCameraWithOpencvSharp.MainFrm.pictureBox1"/>参数
        /// </summary>
        /// <param name="pictureBox"></param>
        public OpenCamera(PictureBox pictureBox)
        {
            PictureBox = pictureBox;

        }
        public void UpdatePictrueImage()
        {
            try
            {

                UpdateImageThreadCallBack_OC = new UpdateImageDelegate(UpdateImage);


                //跟新图片线程
                Thread thread = new Thread(new ThreadStart(UpdateImageThreadCallBack_OC))
                {
                    IsBackground = true
                };
                thread.Start();
                Thread.Sleep(40);
            }
            catch (Exception)
            {

                throw new Exception();
            }

        }
        /// <summary>
        /// 更新图片方法，用于<see cref="UpdateImageThreadCallBack_OC",传递方法/>
        /// </summary>
        private void UpdateImage()
        {
            while (true)
            {
                VideoCapture_OS = InitVideoCapture();
                Mat mat = new Mat();

                if (VideoCapture_OS.Read(mat))
                {
                    Image image = (Image)mat.ToBitmap();
                    PictureBox.Image = image;
                }

            }
        }

        /// <summary>
        /// 初始化摄像头并设置摄像头宽高参数
        /// </summary>
        /// <returns>摄像头调用对象</returns>
        private static VideoCapture InitVideoCapture()
        {
            if (VideoCapture_OS == null)
            {
                VideoCapture_OS = OpenCvSharp.VideoCapture.FromCamera(CaptureDevice.Any);
                VideoCapture_OS.Set(CaptureProperty.FrameWidth, 640);
                VideoCapture_OS.Set(CaptureProperty.FrameHeight, 480);
            }

            return VideoCapture_OS;
        }
    }
}
