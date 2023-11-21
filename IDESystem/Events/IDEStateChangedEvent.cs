using System;
using System.Collections.Generic;
using System.Text;

namespace IOTLib
{
    /// <summary>
    /// 联合DragGameObject使用。不会发送到任意物体。只有CGPrefab的物体才会收到这个消息
    /// </summary>
    public interface IDEStateChangedEvent
    {
        /// <summary>
        /// IDE状态发生变化
        /// </summary>
        /// <param name="enter"></param>
        void OnSceneEditorStateNotify(bool enter);
    }
}
