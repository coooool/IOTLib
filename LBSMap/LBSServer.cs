using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UMOD;
using UnityEngine;

namespace IOTLib
{
    [GameSystem]
    [SystemDescribe(Author = "吴",
       Dependent = "无",
       Describe = "基于GCJ09(谷歌、高德)坐标系的GPS定位服务",
       Name = "LBS定位服务",
       Version = "0.2")]
    public class LBSServer : SingleBaseSystem<LBSServer>
    {
        public override void BindData(DataBehaviour monoBehaviour)
        {
            if(monoBehaviour is ILBSMap)
            {
                base.BindData(monoBehaviour);
            }
            else
            {
                Debug.LogWarning("只能绑定地图服务");
            }
        }

        /// <summary>
        /// 基于CBJ-02坐标系的定位计算一个Unity世界坐标位置
        /// </summary>
        /// <param name="lng">经度(Y)</param>
        /// <param name="lat">纬度(X)</param>
        /// <param name="layerName">指定定位层</param>
        /// <returns>返回的是一个3维坐标，Y坐标为0</returns>
        public static Vector3 CalculateWorldPosition(double lng, double lat, string layerName = "")
        {
            if (string.IsNullOrEmpty(layerName))
            {
                var firstMap = Install.FirstOrDefault();
                if(firstMap != null)
                {
                    return (firstMap as ILBSMap).CalculateWorldPoint(lng, lat);
                }
            }
            else
            {
                foreach(var a in Install)
                {
                    if(a.gameObject.name == layerName)
                    {
                        return (a as ILBSMap).CalculateWorldPoint(lng, lat);
                    }
                }
            }

            throw new System.InvalidOperationException($"场景中不存在位置提供服务");
        }

        /// <summary>
        /// Unity世界位置到GCJ09的经纬度,丢失Y坐标
        /// </summary>
        /// <param name="world_pos">Unity世界坐标的位置</param>
        /// <returns>经纬度</returns>
        public static MapLocation WorldPointToGCJ09Point(Vector3 world_pos, string layerName = "")
        {
            if (string.IsNullOrEmpty(layerName))
            {
                var firstMap = Install.FirstOrDefault();
                if (firstMap != null)
                {
                    return (firstMap as ILBSMap).WorldPointToGCJ09Point(world_pos);
                }
            }
            else
            {
                foreach (var a in Install)
                {
                    if (a.gameObject.name == layerName)
                    {
                        return (a as ILBSMap).WorldPointToGCJ09Point(world_pos);
                    }
                }
            }

            throw new System.InvalidOperationException($"场景中不存在位置提供服务");
        }
    }
}