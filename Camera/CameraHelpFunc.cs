using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace IOTLib
{
    /// <summary>
    /// 摄像头辅助方法
    /// </summary>
    public static class CameraHelpFunc
    {
        /// <summary>
        /// 到观察都模式，该模型通常来自F模式，之后再进入此模式
        /// </summary>
        /// <param name="target"></param>
        public static void ToObserver(GameObject target)
        {
            GameHandleEventSystem.TriggerEvent(CameraHandle.HandleName, PlayerModelObserve.TriggerEventName, new KeyValuePairs(new { Target = target}));
        }

        /// <summary>
        /// 切换到A基本模式，可旋转、平移、行走
        /// </summary>
        /// <param name="target"></param>
        public static void ToAState(KeyValuePairs? args = null) 
        {
            GameHandleEventSystem.TriggerEvent(CameraHandle.HandleName, PlayerModelA.TriggerEventName, args);
        }

        /// <summary>
        /// 切换到2D模式
        /// </summary>
        /// <param name="args"></param>
        public static void To2DState(KeyValuePairs? args = null)
        {
            GameHandleEventSystem.TriggerEvent(CameraHandle.HandleName, PlayerModel_2D.TriggerEventName, args);
        }

        /// <summary>
        /// 到空状态
        /// </summary>
        public static void ToNullState()
        {
            GameHandleEventSystem.TriggerEvent(CameraHandle.HandleName, EmptyState.TriggerEventName);
        }
    }
}
