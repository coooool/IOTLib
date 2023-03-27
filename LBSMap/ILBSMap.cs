using System;
using System.Collections.Generic;
using UnityEngine;

namespace IOTLib
{
    public interface ILBSMap
    {
        /// <summary>
        /// 基于CBJ-02坐标系的定位计算一个Unity世界坐标位置
        /// </summary>
        /// <param name="lng">经度(Y)</param>
        /// <param name="lat">纬度(X)</param>
        /// <returns>返回的是一个3维坐标，Y坐标为0</returns>
        public Vector3 CalculateWorldPoint(double lng, double lat);
        
        /// <summary>
        /// Unity世界位置到GCJ09的经纬度,丢失Y坐标
        /// </summary>
        /// <param name="world_pos"></param>
        /// <returns>经纬度</returns>
        public MapLocation WorldPointToGCJ09Point(Vector3 world_pos);
    }
}
