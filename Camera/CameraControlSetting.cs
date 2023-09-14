using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace IOTLib
{
    [CreateAssetMenu(fileName = ResourceFileName, menuName = "ZN/Camera控制参数")]
    public class CameraControlSetting : ScriptableObject
    {
        private static CameraControlSetting m_setting = null;
        public static CameraControlSetting Setting
        {
            get
            {
                if(m_setting == null)
                {
                    m_setting = Resources.Load<CameraControlSetting>(CameraControlSetting.ResourceFileName);
                    Assert.IsNotNull(m_setting, $"无法加载Resources/{CameraControlSetting.ResourceFileName}的像机控制数据");
                }

                return m_setting;
            }
        }

        public const string ResourceFileName = "CameraControlSetting";

        [Header("移动")]
        [Tooltip("移动时的指数提升系数")]
        public float boost = 3.5f;

        [Header("移动设置")]
        [Tooltip("将相机位置插补到目标位置99%所需的时间。"), Range(0.001f, 1f)]
        public float positionLerpTime = 0.15f;

        [Header("旋转设置")]
        [Tooltip("X=更改鼠标位置。NY=相机旋转的倍增系数。")]
        public AnimationCurve mouseSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));

        [Tooltip("将相机旋转99%内插到目标所需的时间"), Range(0.001f, 1f)]
        public float rotationLerpTime = 0.15f;

        [Tooltip("是否将鼠标输入的Y轴反转为旋转。")]
        public bool invertY = false;

        // 碰撞层
        public LayerMask m_LayerMask = ~0;

        /// <summary>
        /// 控制方法
        /// </summary>
        [Header("其它")]
        public ModelControlTypeEnum ControlMethod = ModelControlTypeEnum.All;

        //球体半径
        public float m_PlayerSphereRadius = 1;

        [Header("动态因子")]
        public bool UseDynamicBoost = false;
        public AnimationCurve DynamicBoostCurve = new AnimationCurve(new Keyframe(0f, 0.0f, 0f, 5f), new Keyframe(10f, 10f, 0f, 0f));

        [Header("F聚焦物体时离地面最小距离")]
        public float FObjectMinGroundDistance = 20;

        /// <summary>
        /// 添加摄像机控制方法
        /// </summary>
        /// <param name="method"></param>
        public void AddCameraControlMethod(ModelControlTypeEnum method)
        {
            ControlMethod |= method;
        }

        /// <summary>
        /// 移除摄像机控制方法
        /// </summary>
        /// <param name="method"></param>
        public void RemoveCameraControlMethod(ModelControlTypeEnum method)
        {
            ControlMethod ^= method;
        }

        /// <summary>
        /// 检查摄像机是否存在目标控制方法
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public bool HasCameraControlMethod(ModelControlTypeEnum method)
        {
            return (ControlMethod & method) == method;
            //return ControlMethod.HasFlag(method);
        }
    }
}