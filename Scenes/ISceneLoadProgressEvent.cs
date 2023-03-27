using System;
using System.Collections.Generic;
using System.Text;

namespace IOTLib.Scenes
{
    /// <summary>
    /// 加载场景时的进度侦听
    /// </summary>
    public interface ISceneLoadProgressEvent : System.IProgress<float>
    {
        /// <summary>
        /// 开始进入加载
        /// </summary>
        void BeginLoad();

        /// <summary>
        /// 加载结束
        /// </summary>
        void LoadClose();
    }
}
