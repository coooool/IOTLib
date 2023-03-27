using System;
using System.Collections.Generic;
using System.Text;

namespace IOTLib.IDESystem
{
    public interface ICGSceneEditorStateNotify
    {
        /// <summary>
        /// 进入编辑器模式
        /// </summary>
        /// <param name="enter">True为进入</param>
        void OnSceneEditorStateNotify(bool enter);
    }
}
