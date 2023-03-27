using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IOTLib.Configure
{
    [System.Serializable]
    public struct ApiRequestData
    {
        public string Name { get; set; }
        public string Method { get; set; }
        public string Url { get; set; }
        public string Header { get; set; }
        public string ContentType { get; set; }
        public string Body { get; set; }
        public ApiRequestData[] Childs { get; set; }

        public override string ToString()
        {
            return string.Format("Name:{0}, Method:{1}, Url:{2}, ContentType:{3}", Name, Method, Url, ContentType);
        }

        /// <summary>
        /// 继承一个属性，但是不会继承Body
        /// </summary>
        /// <param name="data"></param>
        internal void InheritAttribute(ApiRequestData data)
        {
            if (string.IsNullOrEmpty(data.Method) == false)
            {
                Method = data.Method;
            }

            if (string.IsNullOrEmpty(data.Url) == false)
            {
                Url = data.Url;
            }

            if (string.IsNullOrEmpty(data.Header) == false)
            {
                Header = data.Header;
            }

            if (string.IsNullOrEmpty(data.ContentType) == false)
            {
                ContentType = data.ContentType;
            }
        }

        /// <summary>
        /// 分解Header,a=b,c=d以逗号分隔，等号分隔值
        /// </summary>
        /// <param name="kvCallback">结果回调</param>
        public void SpliteHeader(System.Action<string,string> kvCallback)
        {
            if (string.IsNullOrEmpty(Header)) return;

            var heas = Header.Split(',');

            foreach(var a in heas)
            {
                var tmpkv = a.Split('=');
                if(tmpkv.Length != 2)
                {
                    Debug.LogError($"请求Header设置错误:{a},正确的应该为a=b,c=d以逗号分隔，等号分隔值");
                    continue;
                }

                kvCallback?.Invoke(tmpkv[0], tmpkv[1]);
            }
        }

        public static ApiRequestData CreateDefault(string name = null)
        {
            return new ApiRequestData()
            {
                Name = name,
                Method = "GET"
            };
        }
    }
}
