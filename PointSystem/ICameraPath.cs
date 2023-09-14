using System;
using System.Collections.Generic;
using System.Text;

namespace IOTLib
{
    /// <summary>
    /// 提取一个摄像机路径动画
    /// </summary>
    public interface ICameraPath
    {
        /// <summary>
        /// 路径名称
        /// </summary>
        string PathName { get; }
        bool IsPause { get; }

        void Play(params object[] args);
        void Stop();
        void Pause();
    }
}
