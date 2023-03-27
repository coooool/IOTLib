using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMOD;
using IOTLib.Configure.ValueFactory;
using UnityEngine.Assertions;

namespace IOTLib.Configure
{
    /// <summary>
    /// ��������ַӳ���࣬һ����Ŀ�п��԰�����������ַ����ȫ����ʽ����UIϵͳ��HTTPϵͳ������ط����������������
    /// ��ͬ��ƽ̨�Ļ�ȡ��ʽ��һ�¡���֧�ֿ���GUI�������ã�Ҳ�ɼ�ֱ��JSON�ļ�
    /// ��ʾ��
    /// http://abc.com
    /// ��β��/���Ż��Զ����Ƴ�d
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
        /// ��ȡ����������ַ
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
            Assert.IsFalse(string.IsNullOrEmpty(key), "��ȡ����ʱ������һ��nullֵ");

            if (hostMap.TryGetValue(key, out var value))
            {
                return value;
            }

            return string.Empty;
        }
    }
}