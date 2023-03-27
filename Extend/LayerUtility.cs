using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace IOTLib
{
    public class LayerUtility
    {
        public static int cacheMask = -1;
        public static int? cacheGroundLayer;

        public const string GroundLayerName = "Ground";

        /// <summary>
        /// 获取所有层的俺码
        /// </summary>
        public static int AllMaskLayer
        {
            get
            {
                //if(cacheMask == -1)
                //{
                //    for(var i = 0; i < 32; i++)
                //    {
                //        var lname = LayerMask.LayerToName(i);
                //        if (!string.IsNullOrEmpty(lname))
                //        {
                //            cacheMask |= 1 << i;
                //        }
                //    }
                //}

                //return cacheMask;

                return ~0;
            }
        }

        /// <summary>
        /// 获取地面层
        /// </summary>
        /// <returns></returns>
        public static bool GetGroundLayer(out int layer)
        {
            layer = ~0;

            if (cacheGroundLayer.HasValue)
            {
                layer = cacheGroundLayer.Value;
                return true;
            }

            var newLayout = LayerMask.GetMask(GroundLayerName);

            if(newLayout >= 0) {
                cacheGroundLayer = newLayout;
                layer = newLayout;
                return true;
            }

            Debug.LogWarning("项目中似乎不存在Ground的地面层");

            return false;
        }

        /// <summary>
        /// 从所有层中排除指定的目标层。
        /// </summary>
        /// <param name="name">层名</param>
        /// <returns>挑除后的俺码</returns>
        public static int ExcludeLayout(params string[] names)
        {
            int layout = AllMaskLayer;
            int b = LayerMask.GetMask(names);
           
            if(b >= 0)
            {
                return layout &= ~b;
            }

            return layout;
        }
    }
}
