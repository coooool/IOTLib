using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace IOTLib.IDESystem.Handles
{
    internal interface IDragArea
    {
        Rect DragArea { get; }

        /// <summary>
        /// 拖放的目标
        /// </summary>
        /// <param name="target"></param>
        void OnDragPerform(ExportCGPrefab target);
    }
}
