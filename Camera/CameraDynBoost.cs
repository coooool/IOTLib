using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace IOTLib
{
    internal class CameraDynBoost : MonoBehaviour
    {
        Camera m_Camera;
        Vector3? m_MinRayPoint;
        RaycastHit[] m_CacheRayHit = new RaycastHit[8];

        // 增长指数
        public static float Boost { get; private set; } = 1.0f;
        public static float WhellBoost { get; private set; } = 1.0f;

        void Start()
        {
            m_Camera = Camera.main;
            Boost = CameraControlSetting.Setting.boost;
        }

        void FixedUpdate()
        {
            if (Input.anyKey)
            {
                return;
            }

            // 使用动态因子
            if (CameraControlSetting.Setting.UseDynamicBoost)
            {
                var rayCount = Physics.RaycastNonAlloc(m_Camera.transform.position, m_Camera.transform.forward, m_CacheRayHit);

                if (rayCount > 0)
                {
                    float minDistance = 99999;
                    Vector3 selectPoint;
                    int minIndex = 0;

                    for(var i = 0; i < rayCount; i++)
                    {
                        var distance = Vector3.Distance(m_Camera.transform.position, m_CacheRayHit[i].point);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            minIndex = i;
                        }
                    }

                    m_MinRayPoint = m_CacheRayHit[minIndex].point;
                    selectPoint = m_MinRayPoint.Value;

                    var _boost = Mathf.Clamp(minDistance, 1, 99999) / CameraControlSetting.Setting.DBoostScale; //CameraControlSetting.Setting.DynamicBoostCurve.Evaluate(minDistance);

                    Boost = CameraControlSetting.Setting.boost * Mathf.Clamp(_boost, 0.001f, 6);

                    WhellBoost = Vector3.Distance(m_Camera.transform.position, selectPoint) / CameraControlSetting.Setting.DBoostScale; //CameraControlSetting.Setting.MouseWhellCurve.Evaluate(Vector3.Distance(m_Camera.transform.position, target.position));
                    WhellBoost = CameraControlSetting.Setting.mouseWheelSensitivity * WhellBoost;
                }
                else
                {
                    Boost = CameraControlSetting.Setting.boost;
                    WhellBoost = CameraControlSetting.Setting.mouseWheelSensitivity;
                }
            }
        }
    }
}
