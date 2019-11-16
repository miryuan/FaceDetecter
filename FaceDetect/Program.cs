using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Drawing;

namespace FaceDetect
{
    class Program
    {
        /// <summary>
        /// https://blog.csdn.net/it_boy__/article/details/91865726
        /// https://blog.csdn.net/you_big_father/article/details/86088531
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.Title = "快速人脸识别";

            // 视频地址
            VideoCapture capture = new VideoCapture();//@"D:\ca1af880d3653be69ed6d9ce55058c21.mp4"
            capture.Open("rtsp://admin:admin123@192.168.35.190/cam/realmonitor?channel=1&subtype=0");

            Window win = new Window("摄像头");

            //int sleepTime = (int)Math.Round(1000 / 200.0);
            CascadeClassifier cascade = new CascadeClassifier(@"haarcascades\haarcascade_frontalface_alt2.xml");
            //var nestedCascade = new CascadeClassifier(@"..\..\Data\haarcascade_eye_tree_eyeglasses.xml");
            Font font = new Font("宋体", 16, GraphicsUnit.Pixel);
            SolidBrush fontLine = new SolidBrush(Color.Yellow);
            Rect[] faces = null;
            OpenCvSharp.Size sz = new OpenCvSharp.Size(40, 40);

            while (true)
            {
                Mat image = new Mat();
                capture.Read(image);
                if (image.Empty())
                    continue;

                faces = cascade.DetectMultiScale(
                              image: image,
                              scaleFactor: 1.6,
                              minNeighbors: 2,
                              flags: HaarDetectionType.DoRoughSearch | HaarDetectionType.ScaleImage,
                              minSize: sz
                            );

                if (faces.Length > 0) //识别到人脸
                {
                    Bitmap myBitmap = image.ToBitmap();
                    OutputArray frame_gray = image;
                    Cv2.CvtColor(image, frame_gray, ColorConversionCodes.BGR2GRAY);
                    Graphics g = Graphics.FromImage(myBitmap);
                    foreach (Rect face in faces)
                    {
                        g.DrawRectangle(new Pen(Color.YellowGreen, 2), face.X, face.Y, face.Width, face.Height);
                        g.DrawString("人脸", font, fontLine, face.X + face.Width, face.Y);
                    }
                    g.Save();
                    image = myBitmap.ToMat();
                }

                win.Image = image;
                image.Release();//释放
                Cv2.WaitKey(1);
            }


            //while (capture.PosFrames < capture.FrameCount)
            //{
            //    Mat image = new Mat();
            //    capture.Read(image);
            //    //if (image.Empty())
            //    //    break;
            //    faces = cascade.DetectMultiScale(
            //                  image: image,
            //                  scaleFactor: 1.4,
            //                  minNeighbors: 5,
            //                  flags: HaarDetectionType.ScaleImage,
            //                  minSize: new OpenCvSharp.Size(40, 40)
            //                );
            //    if (faces.Length > 0) //没识别到人脸
            //    {
            //        Bitmap myBitmap = image.ToBitmap();
            //        Graphics g = Graphics.FromImage(myBitmap);
            //        foreach (Rect face in faces)
            //        {
            //            g.DrawRectangle(new Pen(Color.YellowGreen, 2), face.X, face.Y, face.Width, face.Height);
            //            //g.DrawString("人脸", font, fontLine, face.X + face.Width, face.Y);
            //        }
            //        g.Save();
            //        image = myBitmap.ToMat();
            //    }
            //    //image.CvtColor(ColorConversionCodes.RGBA2BGR);
            //    win.Image = image;
            //    image.Release();//释放，别等到gc来回收,太占内存
            //    //pictureBox5.BackgroundImage = image.ToBitmap();
            //    Cv2.WaitKey(sleepTime);
            //}

            while (true)
                System.Threading.Thread.Sleep(5000);
        }
    }
}
