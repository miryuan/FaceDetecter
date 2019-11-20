using ArcFaceSharp;
using ArcFaceSharp.ArcFace;
using ArcFaceSharp.Exceptions;
using ArcFaceSharp.Image;
using ArcFaceSharp.Model;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace FaceDetect
{
    class Program
    {
        private static string APP_ID = "nphYwsuxfQhP8UBW1uYTDczkwSG2a7tkEYfZqG2eUdf";
        private static string SDK_KEY = "3h6swqirxGV7j7TtKtUqHdbBWrxKJ4GtYaeGGnbLS3mD";

        /// <summary>
        /// https://blog.csdn.net/it_boy__/article/details/91865726
        /// https://blog.csdn.net/you_big_father/article/details/86088531
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.Title = "快速人脸识别";
            string videoPath = "rtsp://admin:admin123@192.168.35.190/cam/realmonitor?channel=1&subtype=0";

            //RunOnOpenCV(videoPath, "OpenCV摄像头摄像头");
            RunOnArcFace(videoPath, "ArcFace摄像头解析");

            while (true)
                System.Threading.Thread.Sleep(5000);
        }

        /// <summary>
        /// https://github.com/Thxzzzzz/ArcFaceSharp
        /// </summary>
        /// <param name="videoPath"></param>
        /// <param name="videoTitle"></param>
        static void RunOnArcFace(string videoPath, string videoTitle)
        {
            // 视频地址
            VideoCapture capture = new VideoCapture();//@"D:\ca1af880d3653be69ed6d9ce55058c21.mp4"
            capture.Open(videoPath);
            Window win = new Window(videoTitle);
            ArcFaceCore arcFaceCore = new ArcFaceCore(APP_ID, SDK_KEY, ArcFaceDetectMode.VIDEO,
                ArcFaceFunction.FACE_DETECT | ArcFaceFunction.FACE_RECOGNITION | ArcFaceFunction.AGE | ArcFaceFunction.FACE_3DANGLE | ArcFaceFunction.GENDER,
                DetectionOrientPriority.ASF_OP_0_ONLY, 50, 32);

            while (true)
            {
                Mat image = new Mat();
                capture.Read(image);
                if (image.Empty())
                    continue;

                Bitmap bitmap = image.ToBitmap();
                ImageData imageData = ImageDataConverter.ConvertToImageData(bitmap);
                //人脸检测
                MultiFaceModel multiFaceModel = arcFaceCore.FaceDetection(imageData, false);

                // 人脸信息检测 先调用这个接口才能获取以下三个信息
                arcFaceCore.FaceProcess(imageData, multiFaceModel);
                //获取年龄信息
                List<int> ageList = arcFaceCore.GetAge();
                foreach (var item in ageList)
                {
                    Console.WriteLine("Age:" + item);
                }
                // 获取性别信息
                List<int> genderList = arcFaceCore.GetGender();
                foreach (var item in genderList)
                {
                    Console.WriteLine("Sex:" + item);
                }
                // 获取人脸角度信息
                List<Face3DAngleModel> face3DAngleList = arcFaceCore.GetFace3DAngle();
                //foreach (var item in face3DAngleList)
                //{
                //    Console.WriteLine("Face3D:" + item.);
                //}
                //asfSingleFaceInfo 为人脸检测接口返回的人脸信息中的其中一个人脸信息
                AsfSingleFaceInfo asfSingleFaceInfo = new AsfSingleFaceInfo();
                try
                {
                    AsfFaceFeature asfFaceFeature = arcFaceCore.FaceFeatureExtract(imageData, ref asfSingleFaceInfo);
                }
                catch (ResultCodeException e)
                {
                    Console.WriteLine(e.ResultCode);
                    //throw;
                }

                win.Image = bitmap.ToMat();

                // 释放销毁引擎
                arcFaceCore.Dispose();
                // ImageData使用完之后记得要 Dispose 否则会导致内存溢出 
                imageData.Dispose();
                //faceData2.Dispose();
                // BItmap也要记得 Dispose
                //face1.Dispose();
                bitmap.Dispose();
            }
        }


        /// <summary>
        /// OpenCV人脸识别
        /// </summary>
        /// <param name="videoPath"></param>
        /// <param name="videoTitle">OpenCV 摄像头</param>
        static void RunOnOpenCV(string videoPath, string videoTitle)
        {
            // 视频地址
            VideoCapture capture = new VideoCapture();//@"D:\ca1af880d3653be69ed6d9ce55058c21.mp4"
            capture.Open(videoPath);

            Window win = new Window(videoTitle);

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
        }
    }
}
