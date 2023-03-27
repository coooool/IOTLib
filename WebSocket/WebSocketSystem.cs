using System.Collections;
using System.Collections.Generic;
using UMOD;
using UnityEngine;
using UnityEngine.Assertions;

namespace IOTLib
{
    [GameSystem]
    [SystemDescribe(Author = "吴", Dependent = "无", Describe = "WebSocket实例管理", Name = "WebSocket", Version = "0.1")]
    public class WebSocketSystem : BaseSystem
    {
        /// <summary>
        /// 开始连接一个实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        public static void Connection<T>(string url) where T: WSUnityProxy
        {
            Assert.IsFalse(string.IsNullOrEmpty(url), "连接的目标不能为空");

            var sys = SystemManager.GetSystem<WebSocketSystem>();
            var target = sys.GetBindData<T>();

            if (target == null)
            {
                Debug.LogError($"目标代理不存在于场景中,请挂载后再试:{typeof(T).Name}");
            }
            else
            {
                target.Connect(url);
            }
        }

        /// <summary>
        /// 关闭一条链接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Close<T>() where T:WSUnityProxy
        {
            var sys = SystemManager.GetSystem<WebSocketSystem>();
            var target = sys.GetBindData<T>();

            if (target == null)
            {
                Debug.LogError($"目标代理不存在于场景中,请挂载后再试:{typeof(T).Name}");
            }
            else
            {
                target.CloseConnect();
            }
        }

        public static void SendText<T>(string text) where T : WSUnityProxy
        {
            var sys = SystemManager.GetSystem<WebSocketSystem>();
            var target = sys.GetBindData<T>();

            if (target == null)
            {
                Debug.LogError($"目标代理不存在于场景中,请挂载后再试:{typeof(T).Name}");
            }
            else
            {
                target.SendText(text);
            }
        }

        /// <summary>
        /// 尝试获取一个连接
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result">成功返回True</param>
        /// <returns>成功返回True</returns>
        public static bool TryGetConnection<T>(out T result) where T:WSUnityProxy
        {
            result = null;

            var sys = SystemManager.GetSystem<WebSocketSystem>();
            var target = sys.GetBindData<T>();

            if(target == null)
            {
                return false;
            }
            else
            {
                result = target;
                return true;
            }
        }
    }
}