using System;
using System.Threading;
using System.Windows.Forms;

namespace FaceDetect
{
    /// <summary>
    /// 时间管理类
    /// </summary>
    public class TimeManager
    {
        /// <summary>
        /// 更新时间线程方法的委托
        /// </summary>
        private delegate void SetTimeThreadDelegate();
        /// <summary>
        /// 更新时间线程的回调方法
        /// </summary>
        private SetTimeThreadDelegate SetTimeThreadCallBack;

        private delegate void UpdateTimeDelegate(string text);
        private UpdateTimeDelegate UpdateTimeCallBack;

        /// <summary>
        /// 布尔变量用于更新时间线程方法
        /// </summary>
        internal bool TimeBegin = true;

        /// <summary>
        /// 用于获取时间标签
        /// </summary>
        public Label LabelTime { get; private set; }

        public TimeManager(Label label)
        {
            LabelTime = label;
        }
        public TimeManager()
        {

        }
        /// <summary>
        /// 用于窗体加载时间中的方法
        /// </summary>
        internal void TimeWorkForFromLoad()
        {
            //声明更新时间线程回调
            SetTimeThreadCallBack = new SetTimeThreadDelegate(SetTimeForCallBack);
            //
            UpdateTimeCallBack = new UpdateTimeDelegate(UpdateTime);
            //更新时间线程
            Thread thread = new Thread(new ThreadStart(SetTimeThreadCallBack))
            {
                IsBackground = true
            };
            thread.Start();

            Thread.Sleep(40);
        }
        /// <summary>
        /// 更新时间回调方法
        /// </summary>
        private void SetTimeForCallBack()
        {

            while (TimeBegin)
            {
                if (LabelTime.InvokeRequired)
                {
                    LabelTime.Invoke(UpdateTimeCallBack, new object[] { DateTime.Now.ToString() });

                }

            }

        }
        /// <summary>
        /// 更新时间方法
        /// </summary>
        /// <param name="time"></param>
        private void UpdateTime(string time)
        {
            LabelTime.Text = time;
        }

    }
}
