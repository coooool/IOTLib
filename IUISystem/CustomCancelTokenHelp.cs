using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using UnityEngine;

namespace IOTLib.IUISystem
{
    public static class CustomCancelTokenHelp
    {
        /// <summary>
        /// 获取一个Unity对象是否实现了自定义的生命周期
        /// </summary>
        /// <param name="monoBehaviour"></param>
        /// <param name="token"></param>
        /// <returns>True表现成功</returns>
        public static bool GetCustomCancelToken(this object unityObject, out CancellationToken token)
        {
            token = default;

            if(unityObject.GetType().GetCustomAttribute<CustomCancelTokenAttribute>() != null )
            {
                if(unityObject is ICustomCancelToken ict)
                {
                    token = ict.CustomCancelToken;
                    return true;
                }
            }

            return false;
        }
    }
}
