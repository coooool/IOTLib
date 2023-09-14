using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace IOTLib
{
    [CreateAssetMenu(fileName = ResourceFileName, menuName = "ZN/Camera���Ʋ���")]
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
                    Assert.IsNotNull(m_setting, $"�޷�����Resources/{CameraControlSetting.ResourceFileName}�������������");
                }

                return m_setting;
            }
        }

        public const string ResourceFileName = "CameraControlSetting";

        [Header("�ƶ�")]
        [Tooltip("�ƶ�ʱ��ָ������ϵ��")]
        public float boost = 3.5f;

        [Header("�ƶ�����")]
        [Tooltip("�����λ�ò岹��Ŀ��λ��99%�����ʱ�䡣"), Range(0.001f, 1f)]
        public float positionLerpTime = 0.15f;

        [Header("��ת����")]
        [Tooltip("X=�������λ�á�NY=�����ת�ı���ϵ����")]
        public AnimationCurve mouseSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));

        [Tooltip("�������ת99%�ڲ嵽Ŀ�������ʱ��"), Range(0.001f, 1f)]
        public float rotationLerpTime = 0.15f;

        [Tooltip("�Ƿ���������Y�ᷴתΪ��ת��")]
        public bool invertY = false;

        // ��ײ��
        public LayerMask m_LayerMask = ~0;

        /// <summary>
        /// ���Ʒ���
        /// </summary>
        [Header("����")]
        public ModelControlTypeEnum ControlMethod = ModelControlTypeEnum.All;

        //����뾶
        public float m_PlayerSphereRadius = 1;

        [Header("��̬����")]
        public bool UseDynamicBoost = false;
        public AnimationCurve DynamicBoostCurve = new AnimationCurve(new Keyframe(0f, 0.0f, 0f, 5f), new Keyframe(10f, 10f, 0f, 0f));

        [Header("F�۽�����ʱ�������С����")]
        public float FObjectMinGroundDistance = 20;

        /// <summary>
        /// �����������Ʒ���
        /// </summary>
        /// <param name="method"></param>
        public void AddCameraControlMethod(ModelControlTypeEnum method)
        {
            ControlMethod |= method;
        }

        /// <summary>
        /// �Ƴ���������Ʒ���
        /// </summary>
        /// <param name="method"></param>
        public void RemoveCameraControlMethod(ModelControlTypeEnum method)
        {
            ControlMethod ^= method;
        }

        /// <summary>
        /// ���������Ƿ����Ŀ����Ʒ���
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