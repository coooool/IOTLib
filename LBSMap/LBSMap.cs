using System;
using System.Collections;
using System.Collections.Generic;
using UMOD;
using UnityEngine;
using UnityEngine.Serialization;

namespace IOTLib
{ 
    /// <summary>
    /// 基于GCJ-02坐标系的地理位置服务提供
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    [BindSystem(typeof(LBSServer))]
    [Obsolete("建议使用V2版本")]
    public class LBSMap : DataBehaviour, ILBSMap
    {
        //private Vector2D m_originPoint;

        public string m_leftDownPointStr;
        public string m_rightUpPointStr;
        private Vector2D m_leftDownPoint;
        private Vector2D m_rightUpPoint;
    
        private BoxCollider m_boxCollider;
    
        // 虚拟经度平面
        private double m_lngWidth = 0;
        // 虚拟纬度平面
        private double m_latHeight = 0;
    
        // 虚拟平面的偏移
        public Vector2 m_offse = Vector2.one;

        /// <summary>
        /// 基于CBJ-02坐标系的定位计算
        /// </summary>
        /// <param name="lng">经度</param>
        /// <param name="lat">纬度</param>
        public Vector3 CalculateWorldPoint(double lng, double lat)
        {
            var startX = lat - m_leftDownPoint.x;
            var startY = lng - m_leftDownPoint.y;

            // 计算Area中心
            var areaCenter = transform.position + m_boxCollider.center;
            // Area左下角
            var areaLeftDown = new Vector2(areaCenter.x - m_boxCollider.size.x / 2, areaCenter.z - m_boxCollider.size.z / 2);

            var virtualAreaToLBSAreaX = (m_boxCollider.size.x / m_latHeight) ;
            var virtualAreaToLBSAreaY = (m_boxCollider.size.z / m_lngWidth);
        
            var positionX = areaLeftDown.x + (virtualAreaToLBSAreaX * (startY+m_offse.x));
            var positionY = areaLeftDown.y + (virtualAreaToLBSAreaY * (startX+m_offse.y));
        
            return new Vector3((float)positionX, 0, (float)positionY);
        }

        /// <summary>
        /// 世界点到GCJ09的经纬度,无视Y坐标
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public MapLocation WorldPointToGCJ09Point(Vector3 pos)
        {
            // 计算Area中心
            var areaCenter = transform.position + m_boxCollider.center;

            // Area左下角
            var areaLeftDown = new Vector2(areaCenter.x - m_boxCollider.size.x / 2, areaCenter.z - m_boxCollider.size.z / 2);
  
            var startX = (pos.x - areaLeftDown.x) - m_offse.x;
            var startY = (pos.z - areaLeftDown.y) - m_offse.y;

            float scale_x = startX / m_boxCollider.size.x;
            float scale_y = startY / m_boxCollider.size.z;

            var sxxx = scale_x * m_latHeight;
            var syyy = scale_y * m_lngWidth;

            var positionX = m_leftDownPoint.x + syyy;
            var positionY = m_leftDownPoint.y + sxxx;

            return new MapLocation() { lng = positionY, lat = positionX };
        }
    
        private void OnEnable()
        {
            m_boxCollider = GetComponent<BoxCollider>();

            if (!(string.IsNullOrEmpty(m_leftDownPointStr) || string.IsNullOrEmpty(m_rightUpPointStr)))
            {
                // 初始化坐标系
                m_leftDownPoint = Vector2D.Parse(m_leftDownPointStr);
                m_rightUpPoint = Vector2D.Parse(m_rightUpPointStr);

                m_lngWidth = m_rightUpPoint.x - m_leftDownPoint.x;
                m_latHeight = m_rightUpPoint.y - m_leftDownPoint.y;
            }
        }

        private Color __map_box_color = new Color(0, 1, 0, 0.8f);
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = __map_box_color;

            Gizmos.DrawCube(transform.position + m_boxCollider.center, m_boxCollider.size);
        
            var basePoint = transform.position;
            basePoint.y += 50;
        
            var linePosFrom = basePoint;
            linePosFrom.y -= 10;
        
            var linePosTo = basePoint;
            linePosTo.y -= 39;
            Gizmos.DrawLine(linePosFrom, linePosTo );
     
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(basePoint, 10);
        }
    }
}