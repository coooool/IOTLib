using System;
using System.Collections.Generic;
using System.Text;

namespace IOTLib
{
    /// <summary>
    /// 编辑器下的组件值发生改变
    /// </summary>
    public interface IComponentPropertyUpdate
    {
        /// <summary>
        /// 字段更新 
        /// </summary>
        void OnCGFieldUpdate();
    }
}
