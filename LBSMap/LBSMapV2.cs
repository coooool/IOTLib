using System;
using System.Collections;
using System.Collections.Generic;
using UMOD;
using UnityEditor;
using UnityEngine;

namespace IOTLib
{
    [DisallowMultipleComponent]
    [BindSystem(typeof(LBSServer))]
    public class LBSMapV2 : DataBehaviour, ILBSMap
    {
        public const float ExpandDistance = 500000;

        public float m_UnityModelScale = 995803;

        public string m_lnglatText;

        #region Area
        [SerializeField]
        public Vector2D m_leftDownPoint;
        [SerializeField]
        public Vector2D m_leftUpPoint;
        [SerializeField]
        public Vector2D m_rightUpPoint;
        [SerializeField]
        public Vector2D m_rightDownPoint;
        #endregion

        #region Debug
        [NonSerialized]
        public bool m_HasPointInfo = false;

        private double m_lngWidth;
        private double m_latHeight;
        #endregion

        // 当计算的位置无效时，显示在这个位置
        public bool m_OpenIvalidPosReset = false;
        public Vector3 m_InvalidPosition;

        public bool UpdateLntLatData()
        {
            if (string.IsNullOrEmpty(m_lnglatText))
            {
                m_HasPointInfo = false;
                return false;
            }

            try
            {
                var vp = Vector2D.Parse(m_lnglatText);
                var rect = MapUtils.GetLBSPointRect(vp.y, vp.x, LBSMapV2.ExpandDistance);

                m_rightUpPoint = rect[0];
                m_leftUpPoint = rect[1];
                m_leftDownPoint = rect[2];
                m_rightDownPoint = rect[3];
                m_HasPointInfo = true;

                m_lngWidth = m_rightUpPoint.x - m_leftDownPoint.x;
                m_latHeight = m_rightUpPoint.y - m_leftDownPoint.y;

                return true;
            }
            catch (Exception)
            {

            }

            m_HasPointInfo = false;

            return false;
        }

        /// <summary>
        /// 基于CBJ-02坐标系的定位计算一个Unity世界坐标位置
        /// </summary>
        /// <param name="lng">经度(Y)</param>
        /// <param name="lat">纬度(X)</param>
        /// <returns>返回的是一个3维坐标，Y坐标为0</returns>
        public Vector3 CalculateWorldPoint(double lng, double lat)
        {
            if (!m_HasPointInfo)
                throw new System.InvalidOperationException("地图信息不完整");

            var UnityVirtualArea = new Bounds(transform.position, new Vector2(m_UnityModelScale, m_UnityModelScale));

            var startX = lat - m_leftDownPoint.x;
            var startY = lng - m_leftDownPoint.y;

            // 计算Area中心
            var areaCenter = UnityVirtualArea.center;
            // Area左下角
            var areaLeftDown = new Vector2(areaCenter.x - UnityVirtualArea.size.x / 2, areaCenter.z - UnityVirtualArea.size.y / 2);

            var virtualAreaToLBSAreaX = (m_UnityModelScale / m_latHeight);
            var virtualAreaToLBSAreaY = (m_UnityModelScale / m_lngWidth);

            var positionX = areaLeftDown.x + (virtualAreaToLBSAreaX * startY);
            var positionY = areaLeftDown.y + (virtualAreaToLBSAreaY * startX);

            if(LayerUtility.GetGroundLayer(out var gLayout))
            {
                if(Physics.Raycast(
                    new Vector3((float)positionX, 9999999, (float)positionY), 
                    Vector3.down, 
                    out var result, 
                    Mathf.Infinity, 
                    gLayout))
                {
                    return new Vector3((float)positionX, result.point.y, (float)positionY);
                }
                else
                {
                    Debug.LogWarning("转换坐标时无法发现地面");

                    if (m_OpenIvalidPosReset)
                        return m_InvalidPosition;
                }
            }

            var pos = new Vector3((float)positionX, 0, (float)positionY);

            var test_area_pos = pos;
            test_area_pos.y = UnityVirtualArea.center.y;

            if (!m_OpenIvalidPosReset || UnityVirtualArea.Contains(test_area_pos))
            {
                return pos;
            }

            return m_InvalidPosition;
        }

        /// <summary>
        /// 世界点到GCJ09的经纬度,无视Y坐标
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public MapLocation WorldPointToGCJ09Point(Vector3 pos)
        {
            if (m_HasPointInfo == false)
                throw new InvalidOperationException("地图信息不完整");

            var UnityVirtualArea = new Bounds(transform.position, new Vector2(m_UnityModelScale, m_UnityModelScale));

            // 计算Area中心
            var areaCenter = UnityVirtualArea.center;

            // Area左下角
            var areaLeftDown = new Vector2(areaCenter.x - UnityVirtualArea.size.x / 2, areaCenter.z - UnityVirtualArea.size.y / 2);

            var startX = pos.x - areaLeftDown.x;
            var startY = pos.z - areaLeftDown.y;

            double scale_x = startX / UnityVirtualArea.size.x;
            double scale_y = startY / UnityVirtualArea.size.y;

            var x = scale_x * m_latHeight;
            var y = scale_y * m_lngWidth;

            var positionX = m_leftDownPoint.x + y;
            var positionY = m_leftDownPoint.y + x;

            return new MapLocation() { lng = positionY, lat = positionX };
        }

        private void OnEnable()
        {
            if (!m_HasPointInfo)
            {
                if (!UpdateLntLatData())
                {
                    return;
                }
            }

            m_lngWidth = m_rightUpPoint.x - m_leftDownPoint.x;
            m_latHeight = m_rightUpPoint.y - m_leftDownPoint.y;
        }

        private void OnDrawGizmos()
        {
            if (m_HasPointInfo == false) 
                return;
        
            Gizmos.color = Color.green;

            var basePoint = transform.position;

            var linePosFrom = basePoint;

            var linePosTo = basePoint;
            linePosTo.y += 10;
            Gizmos.DrawLine(linePosFrom, linePosTo);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(linePosTo, 1);
        }
    }
}