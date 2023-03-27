using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMOD;
using IOTLib.Configure.ValueFactory;
using UnityEngine.Assertions;

namespace IOTLib.Configure
{
    /// <summary>
    /// 纯主机地址映射类，一个项目中可以包含多个服务地址。完全配置式。在UI系统、HTTP系统及任意地方可以引用这个配置
    /// 不同的平台的获取方式不一致。可支持开发GUI工具配置，也可简单直接JSON文件
    /// 演示：
    /// http://abc.com
    /// 结尾的/符号会自动被移除d
    /// </summary>
    public class HostMap
    {
        private static Dictionary<string, string> hostMap;

        internal static void InitData()
        {
            var hostItems = ConfigureSystem.GetObject<HostData[]>("hosts");

            if (hostItems != null)
            {
                hostMap = new Dictionary<string, string>();

                foreach (var item in hostItems)
                {
                    hostMap.Add(item.name, item.value);
                }

                ValueProviderFactory.RegisterValueProvider(GetConfigureValue);
            }
        }

        /// <summary>
        /// 获取所有主机地址
        /// </summary>
        /// <param name="resultCallback"></param>
        public static void GetAllHost(System.Action<string,string> resultCallback)
        {
            foreach(KeyValuePair<string,string> v in hostMap)
            {
                resultCallback?.Invoke(v.Key, v.Value);
            }
        }

        public static bool AddHost(string key, string host)
        {
            return hostMap.TryAdd(key, host);
        }

        public static bool RemoveHost(string key)
        {
            return hostMap.Remove(key);
        }

        public static bool HasHost(string key)
        {
            return hostMap.ContainsKey(key);
        }

        public static void Clear()
        {
            if (hostMap != null) hostMap.Clear();
        }

        public static string GetHost(string key)
        {
            return hostMap[key];
        }

        public static bool TryGetHost(string key, out string host)
        {
            return hostMap.TryGetValue(key, out host);
        }

        public static string GetConfigureValue(string key)
        {
            Assert.IsFalse(string.IsNullOrEmpty(key), "获取主机时输入了一个null值");

            if (hostMap.TryGetValue(key, out var value))
            {
                return value;
            }

            return string.Empty;
        }
    }
}