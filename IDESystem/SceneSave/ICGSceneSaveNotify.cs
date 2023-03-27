using System;
using System.Collections.Generic;
using System.Text;

namespace IOTLib.IDESystem
{
    /// <summary>
    /// 保存CG场景时通知一次目标，对于一些特殊的类型的设备存储有用
    /// </summary>
    public interface ICGSceneSaveNotify
    {
        void OnSceneSaveNotify();
    }
}
