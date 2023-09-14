using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace IOTLib
{
    /// <summary>
    /// 地面辅助类
    /// </summary>
    public static class GroundUtility
    {
        /// <summary>
        /// 设置位置到地面
        /// </summary>
        public static void SetPositionToGround(this GameObject gameObject, float? minStepHeight)
        {
            SetPositionToGround(gameObject.transform, minStepHeight);
        }

        public static void SetPositionToGround(this Transform transform, float? minStepHeight)
        {
            transform.position= CalculatePositionToGround(transform.position, minStepHeight);
        }

        /// <summary>
        /// 计算一个坐标贴合到地面的Y距离
        /// </summary>
        /// <param name="world_pos"></param>
        /// <param name="minStepHeight">可以为NULL，使用默认的最小统一高度</param>
        /// <returns></returns>
        public static Vector3 CalculatePositionToGround(Vector3 world_pos, float? minStepHeight)
        {
            if (LayerUtility.GetGroundLayer(out var ground_layer))
            {
                var testPoint = world_pos;
                testPoint.y = Camera.main.transform.position.y;
                testPoint.y += 10000;

                if (Physics.Raycast(testPoint, Vector3.down, out var rayInfo, Mathf.Infinity, ground_layer))
                {
                    testPoint = world_pos;

                    var maxY = CameraControlSetting.Setting.FObjectMinGroundDistance;

                    if(minStepHeight.HasValue)
                        maxY = minStepHeight.Value;

                    bool isInRadius = Math.Abs(testPoint.y - rayInfo.point.y) <maxY;

                    if (testPoint.y < rayInfo.point.y)
                    {
                        if (minStepHeight.HasValue)
                            testPoint.y += minStepHeight.Value;
                        else
                            testPoint.y += CameraControlSetting.Setting.FObjectMinGroundDistance*1.1f;

                        return testPoint;
                    }
                    else if (isInRadius)
                    {
                        //testPoint.y += CameraControlSetting.Setting.FObjectMinGroundDistance;

                        if(minStepHeight.HasValue)
                            testPoint.y += minStepHeight.Value;

                        world_pos = testPoint;
                    }
                }
            }
            else Debug.LogWarning("当前场景中似乎不存在地面");

            return world_pos;
        }

        public static Vector3 CalculateFixelPositionToGround(Vector3 world_pos, float minStepHeight)
        {
            if (LayerUtility.GetGroundLayer(out var ground_layer))
            {
                var testPoint = world_pos;
                testPoint.y = Camera.main.transform.position.y;
                testPoint.y += 10000;

                if (Physics.Raycast(testPoint, Vector3.down, out var rayInfo, Mathf.Infinity, ground_layer))
                {
                    world_pos = rayInfo.point;
                    world_pos.y += minStepHeight;
                }
            }
            else Debug.LogWarning("当前场景中似乎不存在地面");

            return world_pos;
        }
    }
}
