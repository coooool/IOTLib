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
        public static void ToObserver(GameObject target, Action Complete = null)
        {
            GameHandleEventSystem.TriggerEvent(
                CameraHandle.HandleName, 
                PlayerModelObserve.TriggerEventName, 
                new KeyValuePairs(new { Target = target, COMPLETE = Complete == null ? CameraHandle.EmptyAction : Complete})
            );
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
        /// 聚集这个设备，摄像机正面对着它
        /// </summary>
        /// <param name="go"></param>
        /// <param name="complete"></param>
        [System.Obsolete("改用FD方法")]
        public static void FAndLookAt(this GameObject go, Action complete = null)
        {
            CameraHandle.F(go, complete, "D");
        }

        public static void FD(this GameObject go, Vector3? forward, Vector3? WorldLeftDirection, float radius = 1.1f, Action Complete =null)
        {
            if (Complete == null) Complete = () => CameraHelpFunc.ToAState();

            var for3 = go.transform.forward;
            var world_left = go.transform.TransformDirection(Vector3.left);

            if (forward.HasValue)
                for3 = go.transform.TransformDirection(forward.Value);

            if (WorldLeftDirection.HasValue)
                world_left = go.transform.TransformDirection(WorldLeftDirection.Value);

            var arg = new { 
                TARGET = go, 
                RADIUS = radius, 
                COMPLETE = Complete, 
                MODEL = "D",
                Forward = for3, WorldLeftDirection = world_left
            };
            
            GameHandleEventSystem.TriggerEvent(
                CameraHandle.HandleName, 
                PlayerModelF.TriggerEventName, 
                new KeyValuePairs(arg)
            );
        }

        /// <summary>
        /// 聚焦一个点
        /// </summary>
        /// <param name="world_pos"></param>
        /// <param name="radius">体积</param>
        public static void FCameraToThis(this Vector3 world_pos, float radius = 1.1f)
        {
            CameraHandle.F(world_pos, null, "A", radius);
        }

        /// <summary>
        /// 到空状态
        /// </summary>
        public static void ToNullState()
        {
            GameHandleEventSystem.TriggerEvent(CameraHandle.HandleName, EmptyState.TriggerEventName);
        }

        /// <summary>
        /// 获取A状态
        /// </summary>
        /// <param name="modelA"></param>
        /// <returns></returns>
        public static bool GetAStateUnit(out PlayerModelA modelA)
        {
            modelA = null;

            var handle = GameHandleSystem.GetFlowGraphFromName(CameraHandle.HandleName);
            if (handle == null)
            {
                Debug.LogError("获取PlayerModelA单元失败，还未初始化");
                return false;
            }

            modelA = handle.GraphRef.GetUnit<PlayerModelA>();

            return modelA != null;
        }
    }
}
