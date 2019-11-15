using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Threading;

namespace FaceDetect
{
    class Program
    {
        /// <summary>
        /// https://blog.csdn.net/it_boy__/article/details/91865726
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.Title = "快速人脸识别";

            // 视频地址
            VideoCapture capture = new VideoCapture(@"D:\Download\迅雷下载\西部世界.Westworld.S02E08.1080p精校版-天天美剧字幕组.mp4");
            Window win = new Window();
            int sleepTime = (int)Math.Round(1000 / capture.Fps);

            while (capture.PosFrames < capture.FrameCount)
            {
                Mat image = new Mat();
                capture.Read(image);
                win.Image = image;
                //pictureBox5.BackgroundImage = image.ToBitmap();
                Cv2.WaitKey(sleepTime);
                image.Release();//释放，别等到gc来回收,太占内存
            }

            while (true)
                Thread.Sleep(5000);
        }
    }
}
