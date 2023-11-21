using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace IOTLib
{
    public interface IDEPropertyEvent
    {
        /// <summary>
        /// 变换更新
        /// </summary>
        void OnTransformUpdate(Transform transform);
    }
}
