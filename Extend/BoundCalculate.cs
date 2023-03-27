using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Pool;

namespace IOTLib.Extend
{
    /// <summary>
    /// 计算包围体实用程序
    /// </summary>
    public class BoundsUtility
    {
        public static Bounds CalculateBound(IEnumerable<GameObject> points)
        {
            return CalculateBound(points.Select(p => p.transform.position));
        }

        public static Bounds CalculateBound(IEnumerable<Transform> points)
        {
            return CalculateBound(points.Select(p => p.position));
        }

        public static Rect CalculateBound(IEnumerable<RectTransform> points)
        {
            return CalculateBound(points.Select(p => p.anchoredPosition));
        }

        /// <summary>
        /// 计算一批点位的包围体
        /// </summary>
        /// <param name="points">点位</param>
        /// <returns></returns>
        public static Bounds CalculateBound(IEnumerable<Vector3> points)
        {
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float minZ = float.MaxValue;

            float maxX = float.MinValue;
            float maxY = float.MinValue;
            float maxZ = float.MinValue;

            foreach (Vector3 point in points)
            {
                if (point.x < minX)
                {
                    minX = point.x;
                }
                
                if (point.x > maxX)
                {
                    maxX = point.x;
                }

                if (point.y < minY)
                {
                    minY = point.y;
                }
                
                if (point.y > maxY)
                {
                    maxY = point.y;
                }

                if (point.z < minZ)
                {
                    minZ = point.z;
                }
                
                if (point.z > maxZ)
                {
                    maxZ = point.z;
                }
            }

            var newBound = new Bounds();
            newBound.min = new Vector3(minX, minY, minZ);
            newBound.max = new Vector3(maxX, maxY, maxZ);

            return newBound;
        }

        public static Rect CalculateBound(IEnumerable<Vector2> points)
        {
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;

            foreach (Vector2 point in points)
            {
                if (point.x < minX)
                {
                    minX = point.x;
                }
                else if (point.x > maxX)
                {
                    maxX = point.x;
                }

                if (point.y < minY)
                {
                    minY = point.y;
                }
                else if (point.y > maxY)
                {
                    maxY = point.y;
                }
            }

            return new Rect(new Vector2(minX, minY), new Vector2(maxY, maxY));
        }

        /// <summary>
        /// 计算一个物体的包围体，也会包含子物体
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static Bounds GetBoundsWithChildren(GameObject gameObject)
        {
            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();

            Bounds bounds = renderers.Length > 0 ? renderers[0].bounds : new Bounds();

            for (int i = 1; i < renderers.Length; i++)
            {
                if (renderers[i].enabled)
                {
                    if (renderers[i].gameObject.name.StartsWith('.'))
                        continue;

                    bounds.Encapsulate(renderers[i].bounds);
                }
            }

            return bounds;
        }
    }
}