using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading;
using IOTLib.Configure;
using IOTLib.Configure.ValueFactory;
using Newtonsoft.Json;

namespace IOTLib
{
    /// <summary>
    /// 基于配置的HTTP构建器
    /// </summary>
    public class ConfigHttpBuild : IRequestFactory
    {
        protected CancellationToken? CancellationToken { get; set; }
        protected string RequestName { get; set; }
        protected KeyValuePairs? Parameter { get; set; } = null;
        protected string? Body { get; set; } = null;
        protected string? ContentType { get; set; }

        public ConfigHttpBuild(string name)
        {
            UnityEngine.Assertions.Assert.IsTrue(string.IsNullOrEmpty(name), "接口名称不能为空");
            RequestName = name;
        }

        /// <summary>
        /// 从配置文件构建一个请求
        /// </summary>
        /// <param name="name">接口名称</param>
        /// <param name="cancelToken">生命周期，没有为全局，建议设置</param>
        public ConfigHttpBuild(string name, CancellationToken? cancelToken) : this(name)
        {
            CancellationToken = cancelToken;
        }

        /// <summary>
        /// 根据匿名对象填充数据 
        /// </summary>
        /// <param name="args"></param>
        public ConfigHttpBuild SetParameter(System.Object args)
        {
            this.Parameter = new KeyValuePairs(args);
            return this;
        }

        public ConfigHttpBuild SetParameter(KeyValuePairs? args)
        {
            this.Parameter = args;
            return this;
        }

        /// <summary>
        /// 设置请求体,会从SetParameter中的源数据进行数据填充
        /// </summary>
        /// <param name="data"></param>
        public ConfigHttpBuild SetBody(string data)
        {
            Body = data;
            return this;
        }

        /// <summary>
        /// 设置请求数据类型
        /// </summary>
        /// <param name="type"></param>
        public ConfigHttpBuild SetContentType(string type)
        {
            ContentType = type;
            return this;
        }

        /// <summary>
        /// 可传入匿名类型，内部自动序列化。
        /// </summary>
        /// <param name="data"></param>
        /// <exception cref="System.InvalidOperationException"></exception>
        public ConfigHttpBuild SetBody(object data)
        {
            try
            {
                SetBody(JsonConvert.SerializeObject(data));
            }
            catch(Exception ex)
            {
                Debug.LogError($"设置Body失败：{ex.Message}");
                throw new System.InvalidOperationException("设置Body失败");
            }

            return this;
        }

        public UnityWebRequest CreateRequest()
        {
            // 获取API数据
            var req = ApiMap.GetApiData(RequestName, this.Parameter);

            var httpBuild = new HttpRequestBuild(req.Url, req.Method);

            req.SpliteHeader((k, v) =>
            {
                if(ValueProviderFactory.GetValueFromProvider(v, this.Parameter, out var bindVal))
                {
                    httpBuild.SetCustomHeader(k, bindVal);
                }
                else
                {
                    httpBuild.SetCustomHeader(k, v);
                }
            });

            // 设置了自定义的Body
            if (!string.IsNullOrEmpty(Body))
            {
                //string tmpBody = Body;
                //ApiMap.ParseUrlParameter(ref tmpBody, this.Parameter, null);
                httpBuild.SetBody(Body);
            }

            if(string.IsNullOrEmpty(ContentType) == false)
            {
                httpBuild.SetContentType(ContentType);
            }

            return httpBuild.Build();
        }

        void IRequestFactory.SetRequestNodeData(ref EasyRequest request)
        {
            if (CancellationToken.HasValue)
                request.CancellationToken = CancellationToken.Value;
        }

        public string GetRequestName()
        {
            return RequestName;
        }

        public IRequestFactory SetCancellationToken(CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;
            return this;
        }
    }
}
