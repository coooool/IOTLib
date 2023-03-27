using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace IOTLib
{
    public static class PhysicsFunc
    {
        static RaycastHit[] Cache_RayHits = new RaycastHit[6];

        /// <summary>
        /// 一个世界坐标的向量是否在摄像机内可见?
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="position"></param>
        /// <returns>True为可见，反之。</returns>
        public static bool PointInCameraView(this Camera camera, Vector3 position)
        {
            return PointInCameraView(position, camera);
        }

        /// <summary>
        /// 一个世界坐标的向量是否在摄像机内可见?
        /// </summary>
        /// <param name="position">矢量</param>
        /// <returns>True为可见，反之。</returns>
        public static bool PointInCameraView(Vector3 position)
        {
            return PointInCameraView(position, Camera.main);
        }

        public static bool PointInCameraView(Vector3 position, Camera camera)
        {
            var cpsub = position - camera.transform.position;
            var cpdot = Vector3.Dot(cpsub.normalized, camera.transform.forward);

            var viewPos = camera.WorldToViewportPoint(position);

            if (cpdot < 0 || viewPos.x < 0 || viewPos.x > 1)
            {
                return false;
            }
            else if (cpdot < 0 || viewPos.y < 0 || viewPos.y > 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 计算一个位置到地面,会产生贴合到地面的效果
        /// </summary>
        /// <param name="world_pos">坐标位置</param>
        /// <returns></returns>
        public static Vector3 CalculateGroundPosition(Vector3 world_pos, float offset = 0)
        {
            int ground;
            var minDistance = float.MaxValue;
            Vector3 selectPoint = Vector3.zero;
            bool havePoint = false;

            LayerUtility.GetGroundLayer(out ground);
           

            var hitCount = Physics.RaycastNonAlloc(world_pos + Camera.main.transform.TransformDirection(Vector3.up), Vector3.down,
             Cache_RayHits, Mathf.Infinity, ground);

            if (hitCount > 0)
            {
                for (var i = 0; i < hitCount; i++)
                {
                    if (Cache_RayHits[i].distance < minDistance)
                    {
                        havePoint = true;
                        minDistance = Cache_RayHits[i].distance;
                        selectPoint = Cache_RayHits[i].point;
                    }
                }
            }

            if (!havePoint)
            {
                if(Physics.Raycast( 
                    Camera.main.ScreenPointToRay(Input.mousePosition),
                    out var cameraHit, 
                    Mathf.Infinity, ground)
                )
                {
                    var newWorldPos = world_pos;
                    newWorldPos.y = cameraHit.point.y + 1;

                    hitCount = Physics.RaycastNonAlloc(newWorldPos,
                        Vector3.down, Cache_RayHits, Mathf.Infinity, ground);

                    if (hitCount > 0)
                    {
                        for (var i = 0; i < hitCount; i++)
                        {
                            if (Cache_RayHits[i].distance < minDistance)
                            {
                                havePoint = true;
                                minDistance = Cache_RayHits[i].distance;
                                selectPoint = Cache_RayHits[i].point;
                            }
                        }
                    }
                }
            }

            if (havePoint)
            {
                world_pos.y = selectPoint.y + offset;
            }

            return world_pos;
        }
    }
}
