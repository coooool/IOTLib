using System;
using System.Collections.Generic;
using System.Text;

namespace IOTLib
{
    /// <summary>
    /// 鼠标与键盘锁
    /// </summary>
    public class IOLockState
    {
        public static bool IsPointerOverGameObject { get; set; }
        public static bool IsPointerHoverGameObject { get; set; }

        public static bool CGEditorMouseInDragWindow { get; internal set; }

        public static bool OtherIsUse { get; set; }

        public static bool AllLock()
        {
            return IsPointerOverGameObject || IsPointerHoverGameObject || CGEditorMouseInDragWindow || OtherIsUse;

        }

        public static bool CGEditorLock()
        {
            return CGEditorMouseInDragWindow || OtherIsUse;
        }
    }
}
