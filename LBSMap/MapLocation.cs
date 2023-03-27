using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IOTLib
{
    /// <summary>
    /// 经纬度实体
    /// </summary>
    public struct MapLocation
    {
        /// <summary>
        /// 纬度(X)
        /// </summary>
        public double lat { get; set; }

        /// <summary>
        /// 经度(Y)
        /// </summary>
        public double lng { get; set; }

        public override string ToString()
        {
            return string.Format("经纬度:{0},{1}", lat, lng);
        }
    }
}