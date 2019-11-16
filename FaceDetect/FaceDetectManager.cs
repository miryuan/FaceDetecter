using OpenCvSharp;
using OpenCvSharp.Face;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MyFaceDetect
{
    /// <summary>
    /// 
    /// </summary>
    public class FaceDetectManager
    {
        private List<ImageInfo> images;
        //groupId -> name 快速查询
        private Dictionary<int, string> faceDic;
        private FaceRecognizer faceRecognizer;

        internal List<ImageInfo> Images { get => images; set => images = value; }
        public Dictionary<int, string> FaceDic { get => faceDic; set => faceDic = value; }
        public FaceRecognizer FaceRecognizer { get => faceRecognizer; set => faceRecognizer = value; }

        public FaceDetectManager()
        {
            Images = new List<ImageInfo>();
            FaceDic = new Dictionary<int, string>();
            GetFaceRecognizer();

        }
        /// <summary>
        /// 获得一个训练好的model
        /// </summary>
        public void GetFaceRecognizer()
        {
            //获得已有的人脸信息库，信息存在images 和 faceDic中
            GetImageInfos();

            //使用了FisherFaceRecognizer类型的人脸识别器
            faceRecognizer = FisherFaceRecognizer.Create();

            //进行人脸数据的训练 每张图片及它的标识Id
            //List<Mat> mats = new List<Mat>();
            //List<int> labs = new List<int>();
            //for(int i = 0; i < images.Count; i++)
            //{
            //    mats.Add(images[i].Image);
            //    labs.Add(images[i].ImageGroupId);
            //}
            //faceRecognizer.Train(mats, labs);
            faceRecognizer.Train(Images.Select(x => x.Image), Images.Select(x => x.ImageGroupId));
        }
        /// <summary>
        /// 获取已有的人脸信息
        /// </summary>
        public void GetImageInfos()
        {
            int imageId;
            DirectoryInfo directory = new DirectoryInfo(@"..\..\Images");
            foreach (DirectoryInfo dir in directory.GetDirectories())
            {
                imageId = 0;
                string[] idAndName = dir.ToString().Split('_');
                int groupId = int.Parse(idAndName[0]);
                foreach (FileInfo file in dir.GetFiles())
                {
                    Images.Add(new ImageInfo
                    {
                        Image = new Mat(file.FullName, ImreadModes.Grayscale),
                        ImageGroupId = groupId,
                        ImageId = imageId++,

                    });
                }
                FaceDic.Add(groupId, idAndName[1]);
            }
        }

        /// <summary>
        /// 更新人脸信息库
        /// </summary>
        /// <param name="name"></param>
        /// <param name="face"></param>
        public void UpdateImagesInfos(string name, Mat face)
        {
            //信息库存在这个人名
            if (FaceDic.ContainsValue(name))
            {
                int groupId = FaceDic.FirstOrDefault(q => q.Value.Equals(name)).Key;
                string dir = "..\\..\\Images\\" + groupId + "_" + name;
                int count = 0;
                foreach (FileInfo file in new DirectoryInfo(dir).GetFiles())
                {
                    count++;
                }
                if (count < 3)
                {
                    face.SaveImage(dir + "\\" + count + ".png");
                    images.Add(new ImageInfo
                    {
                        Image = face,
                        ImageGroupId = groupId,
                        ImageId = count,

                    });
                }
                else
                {
                    MessageBox.Show("样本提取完成，不需要重复提取该人脸图像");
                    return;
                }
            }
            //不存在，则加入该人脸,并更新images中的数据和faceDic
            else
            {
                string path = "..\\..\\Images\\";
                string[] list = System.IO.Directory.GetDirectories(path);
                int dirLen = list.Length;
                Directory.CreateDirectory(path + dirLen + "_" + name);
                face.SaveImage(path + dirLen + "_" + name + "\\" + 0 + ".png");
                faceDic.Add(dirLen, name);
                images.Add(new ImageInfo
                {
                    Image = face,
                    ImageGroupId = dirLen,
                    ImageId = 0,

                });
            }
            MessageBox.Show("更新成功");
        }
        public bool IsContainsValue(string name)
        {
            Dictionary<int, string>.ValueCollection values = FaceDic.Values;
            foreach (string value in values)
            {
                if (value.Equals(name))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 获得人脸(矩形框内的)头像,灰度化，均匀化
        /// </summary>
        /// <param name="srcImg">参数是原始图片，从摄像头下抓取的</param>
        /// <param name="faces">人脸矩形框</param>
        /// <returns>返回人脸信息</returns>

        public List<Mat> GetFaces(Mat srcImg, Rect[] faces)
        {
            //Rect[] faces = GetFaceRects(srcImg);
            List<Mat> dstFaces = new List<Mat>();
            foreach (Rect face in faces)
            {
                Mat matFace = new Mat(srcImg, face);
                Cv2.CvtColor(matFace, matFace, ColorConversionCodes.BGR2GRAY);
                Cv2.Resize(matFace, matFace, new OpenCvSharp.Size(100, 100));
                Cv2.EqualizeHist(matFace, matFace);
                dstFaces.Add(matFace);
            }
            return dstFaces;
        }

        /// <summary>
        /// 根据模型预测人脸,返回人脸所对应的人名  Predict()
        /// </summary>
        /// <param name="curFaces">经灰度化，均匀化处理的人脸头像</param>
        /// <returns></returns>
        public List<string> PredictFace(List<Mat> curFaces)
        {
            List<string> names = new List<string>();
            for (int i = 0; i < curFaces.Count; i++)
            {
                int groupId = -1;
                groupId = faceRecognizer.Predict(curFaces[i]);
                string desName;
                FaceDic.TryGetValue(groupId, out desName);
                names.Add(desName);
            }
            return names;
        }

        /// <summary>
        /// 用于检测人脸，返回人脸矩形框
        /// </summary>
        /// <param name="srcImage">原始输入图像，摄像头采集的</param>
        /// <returns>返回人脸矩形框</returns>
        public Rect[] GetFaceRects(Mat srcImage)
        {
            Mat grayImage = new Mat();
            Cv2.CvtColor(srcImage, grayImage, ColorConversionCodes.BGR2GRAY);
            Cv2.EqualizeHist(grayImage, grayImage);

            CascadeClassifier cascade = new CascadeClassifier(@"haarcascades\haarcascade_frontalface_alt.xml");

            Rect[] faces = cascade.DetectMultiScale(
                image: grayImage,
                scaleFactor: 1.1,
                minNeighbors: 2,
                flags: HaarDetectionType.DoRoughSearch | HaarDetectionType.ScaleImage,
                minSize: new OpenCvSharp.Size(30, 30)
            );

            return faces;
        }

        /// <summary>
        /// 将得到的矩形框和人名画出来
        /// </summary>
        /// <param name="faces">人脸矩形框</param>
        /// <param name="grab">原始的图像</param>
        /// <param name="names">预测得到的人脸对应的名字</param>
        /// <returns></returns>
        public void ShowFaceRects(Rect[] faces, Bitmap grab, List<string> names)
        {
            Graphics g = Graphics.FromImage(grab);
            //Bitmap resBitmap = new Bitmap(100,100);
            //Random rnd = new Random();
            int count = 0;
            Font font = new Font("宋体", 16, GraphicsUnit.Pixel);
            SolidBrush fontLine = new SolidBrush(Color.Yellow);
            foreach (Rect face in faces)
            {
                g.DrawRectangle(new Pen(Color.YellowGreen, 2), face.X, face.Y, face.Width, face.Height);
                //OpenCv自带的PutText函数不能显示中文
                //Scalar color = new Scalar(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
                //Cv2.Rectangle(grab, face, color);
                //Cv2.PutText(grab,names[count++],face.TopLeft,HersheyFonts.HersheySimplex,0.8, new Scalar(255, 23, 0));
                float xPos = face.X + (face.Width / 2 - (names[count].Length * 14) / 2);
                float yPos = face.Y - 21;
                g.DrawString(names[count++], font, fontLine, xPos, yPos);
            }
        }
    }
}
