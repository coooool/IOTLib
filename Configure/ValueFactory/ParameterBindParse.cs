using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using static IOTLib.Configure.ValueFactory.ValueProviderFactory;
using static UnityEngine.Networking.UnityWebRequest;

namespace IOTLib.Configure.ValueFactory
{
    /// <summary>
    /// 绑定参数辅助类
    /// </summary>
    internal struct ParameterBindParse
    {
        List<KeyValuePair<string, string>> cachePargrame;
        // 绑定成功的数量
        public int BindSuccessCount => cachePargrame.Count;

        public ParameterBindParse(int allocArgSize)
        {
            cachePargrame= new List<KeyValuePair<string, string>>(allocArgSize);
        }

        bool HasKey(string key)
        {
            return cachePargrame.Any(p => p.Key == key);
        }

        public int BindValuFromKeyPairs(KeyValuePairs values, KeyValuePair<string,string>[] vars)
        {
            var result = 0;

            foreach(var pair in vars)
            {
                if (HasKey(pair.Key)) continue;

                if (values.TryGetValue(pair.Key, out var cusData))
                {
                    cachePargrame.Add(new KeyValuePair<string, string>(pair.Key, cusData.ToString()));
                    result++;
                }
            }
            
            return result;
        }

        public int BindValueFromFuncDelegate(ValueProviderFuncDelegate valueProvider, KeyValuePair<string, string>[] vars)
        {
            int result = 0;

            foreach (var pair in vars)
            {
                if (HasKey(pair.Key)) continue;

                var value = valueProvider(pair.Key);

                if (!string.IsNullOrEmpty(value))
                {
                    cachePargrame.Add(new KeyValuePair<string, string>(pair.Key, value));
                    result++;
                }
            }

            return result;
        }

        public int BindValueFromValueProviderNode(IConfigureValueProvider valueProvider, KeyValuePair<string, string>[] vars)
        {
            int result = 0;

            foreach (var pair in vars)
            {
                if (HasKey(pair.Key)) continue;

                var value = valueProvider.GetConfigureValue(pair.Key);

                if (!string.IsNullOrEmpty(value))
                {
                    cachePargrame.Add(new KeyValuePair<string, string>(pair.Key, value));
                    result++;
                }
            }

            return result;
        }

        public int BindValueFromDefaultValue(KeyValuePair<string, string>[] vars)
        {
            int result = 0;

            foreach (var pair in vars)
            {
                if (HasKey(pair.Key)) continue;

                if (!string.IsNullOrEmpty(pair.Value))
                {
                    cachePargrame.Add(new KeyValuePair<string, string>(pair.Key, pair.Value));

                    result++;
                }
            }

            return result;
        }

        public string GetValueAtIndex(int index)
        {
            Assert.IsFalse(index < cachePargrame.Count, $"获取的参数越界:{index},实际数量:{cachePargrame.Count}");

            return cachePargrame[index].Value;
        }
       
        // 转换为调用参数
        public string[] ValueParameters()
        {
            if (cachePargrame.Count == 0) return Array.Empty<string>();

            return cachePargrame.Select(a => a.Value).ToArray();
        }

        public string GetDefaultValue(string name)
        {
            foreach (var pair in cachePargrame)
            {
                if(pair.Key == name)
                {
                    return pair.Value;
                }
            }

            return string.Empty;
        }
    }
}
