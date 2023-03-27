using IOTLib.Extend;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IOTLib
{
    internal class CameraPhysics
    {
        private static Collider[] m_PlayerhitCollider = new Collider[8];
        private static Collider[] m_ColliderCache2 = new Collider[8];

        private static RaycastHit[] m_PlayerRaycastHit = new RaycastHit[4];

        public static bool OverlapTest(UnityEngine.Camera camera, ref Vector3 direction)
        {
            var result = false;

            var forward = camera.transform.forward;
            var right = camera.transform.right;
            var up = camera.transform.up;

            if (Physics.RaycastNonAlloc(camera.transform.position, forward, m_PlayerRaycastHit, CameraControlSetting.Setting.m_PlayerSphereRadius, CameraControlSetting.Setting.m_LayerMask) > 0)
            {
                if (direction.z > 0)
                {
                    result = true;
                    direction.z = 0;
                }
            }
            else if (Physics.RaycastNonAlloc(camera.transform.position, -forward, m_PlayerRaycastHit, CameraControlSetting.Setting.m_PlayerSphereRadius, CameraControlSetting.Setting.m_LayerMask) > 0)
            {
                if (direction.z < 0)
                {
                    result = true;
                    direction.z = 0;
                }
            }

            if (Physics.RaycastNonAlloc(camera.transform.position, right, m_PlayerRaycastHit, CameraControlSetting.Setting.m_PlayerSphereRadius, CameraControlSetting.Setting.m_LayerMask) > 0)
            {
                if (direction.x > 0) { direction.x = 0; result = true; }
            }
            else if (Physics.RaycastNonAlloc(camera.transform.position, -right, m_PlayerRaycastHit, CameraControlSetting.Setting.m_PlayerSphereRadius, CameraControlSetting.Setting.m_LayerMask) > 0)
            {
                if (direction.x < 0) { direction.x = 0; result = true; }
            }

            if (Physics.RaycastNonAlloc(camera.transform.position, up, m_PlayerRaycastHit, CameraControlSetting.Setting.m_PlayerSphereRadius, CameraControlSetting.Setting.m_LayerMask) > 0)
            {
                if (direction.y > 0) { direction.y = 0; result = true; }
            }
            else if (Physics.RaycastNonAlloc(camera.transform.position, -up, m_PlayerRaycastHit, CameraControlSetting.Setting.m_PlayerSphereRadius, CameraControlSetting.Setting.m_LayerMask) > 0)
            {
                if (direction.y < 0) { direction.y = 0; result = true; }
            }

            return result;
        }

        public static bool LerpOverlapTest(UnityEngine.Camera camera)
        {
            int num = Physics.OverlapSphereNonAlloc(camera.transform.position, CameraControlSetting.Setting.m_PlayerSphereRadius, m_PlayerhitCollider, CameraControlSetting.Setting.m_LayerMask);

            if (num > 0)
            {
                //var collider = m_PlayerhitCollider[0];
                //var closestPoint = collider.ClosestPointOnBounds(camera.transform.position);
               
                return true;
            }

            return false;
        }

        #region 排斥算法
        // 外部体积算法
        static Vector3 OutsideBoundsVolume(Vector3 worldPos, float objectRadius, Bounds bounds, Vector3? upAxis = null)
        {
            if (upAxis.HasValue == false)
                upAxis = Vector3.up;

            var targetPosition = worldPos;

            Vector3 closestPoint = bounds.ClosestPoint(worldPos);
            float distance = Vector3.Distance(worldPos, closestPoint);

            float center_offset = Vector3.Distance(worldPos, bounds.center);

            Vector3 direction = (worldPos - closestPoint).normalized;

            // 禁止往下偏移
            if (Vector3.Dot((closestPoint - bounds.center).normalized, upAxis.Value) < 0)
            {
                direction = bounds.center - worldPos;
                closestPoint = bounds.center + (direction.normalized * bounds.extents.magnitude);
                var modelRadios = (closestPoint - bounds.center).normalized * (objectRadius);

                return closestPoint + modelRadios;
            }
            else
            {
                targetPosition += direction * ((objectRadius + 0.1f) - distance);
            }

            return targetPosition;
        }

        static Vector3 InBoundsVolume(Vector3 worldPos, float objectRadius, Bounds bounds, Vector3? upAxis = null)
        {
            var direction = worldPos - bounds.center;

            if(!upAxis.HasValue) upAxis= Vector3.up;

            // 禁止往下偏移
            if (Vector3.Dot(direction.normalized, upAxis.Value) < 0)
            {
                direction = bounds.center - worldPos;
            }
            else
            {
                direction = worldPos - bounds.center;
            }

            var closestPoint = bounds.center + (direction.normalized * bounds.extents.magnitude);

            var modelRadios = (closestPoint - bounds.center).normalized * (objectRadius);

            return closestPoint + modelRadios;
        }

        // 内部体积算法
        static Vector3 CalculateBestPoint(Vector3 worldPos, float objectRadius, IEnumerable<Bounds> allbounds, int depth, Vector3? upAxis = null)
        {
            if (upAxis.HasValue == false)
                upAxis = Vector3.up;

            Bounds bounds = new Bounds();

            foreach (var a in allbounds) bounds.Encapsulate(a);

            var testClosestPoint = bounds.ClosestPoint(worldPos);
            var distance = Vector3.Distance(worldPos, testClosestPoint);

            var result = Vector3.zero;
            // 在体积外面
            if (distance > 0)
            {
                result = OutsideBoundsVolume(worldPos, objectRadius, bounds, upAxis);
            }
            else
            {
                result = InBoundsVolume(worldPos, objectRadius, bounds, upAxis);
            }


            if (depth > 200)
            {
                Debug.LogError($"算法出现异常，出现了重复的数据");
                return result;
            }

            var lay = LayerUtility.ExcludeLayout("Ground");

            int OverlapCount = Physics.OverlapSphereNonAlloc(result, objectRadius, m_ColliderCache2, lay);

            if (OverlapCount > 0)
            {
                result = CalculateBestPoint(result, objectRadius, m_ColliderCache2.Select(p => p.bounds).Take(OverlapCount), ++depth, upAxis);
            }

            return result;
        }
        #endregion

        /// <summary>
        /// 计算一个像机目标位置的最佳点物理点，避免穿透，
        /// 新的位置能容纳摄像机1.2个大小
        /// </summary>
        /// <param name="targetPos"></param>
        /// <param name="lockYAxis">锁定Y轴不变？</param>
        /// <returns></returns>
        public static Vector3 CalculateCameraBestPoint(Vector3 targetPos)
        {
            var cameraRadius = CameraControlSetting.Setting.m_PlayerSphereRadius;

            var count = Physics.OverlapSphereNonAlloc(targetPos, cameraRadius, m_PlayerhitCollider, LayerUtility.ExcludeLayout("Ground"));

            if (count == 0)
            {
                return new Vector3(targetPos.x, Mathf.Max(targetPos.y, cameraRadius), targetPos.z);
            }
            else if (count == 1)
            {
                if (m_PlayerhitCollider[0].transform.position == targetPos)
                    return new Vector3(targetPos.x, Mathf.Max(targetPos.y, cameraRadius), targetPos.z);
            }
 
            Bounds bounds = new Bounds();

            for (var i = 0; i < count; i++) bounds.Encapsulate(m_PlayerhitCollider[i].bounds);

            var newPosition = CalculateBestPoint(
                targetPos,
                cameraRadius, m_PlayerhitCollider.Select(p => p.bounds).Take(count),
                1,
                Camera.main.transform.TransformDirection(Vector3.up)
            );

            return new Vector3(newPosition.x, Mathf.Max(newPosition.y, cameraRadius), newPosition.z);
        }
    }
}