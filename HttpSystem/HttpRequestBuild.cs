using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace IOTLib
{
    /// <summary>
    /// HTTP 请求数据构建器
    /// 实际发送数据由HTTPSystem推进，该类型只构造一个等侍请求的数据
    /// GET与POST的默认请求的内容编码为application/json
    /// 授权标头由用户系统注册在HTTPSYSTEM里的请求拦截器提供
    /// </summary>
    public class HttpRequestBuild : IRequestFactory
    {
        // 为空时默认GET
        private string m_Method;

        /// <summary>
        /// HTTP请求类型(GET,POST,PUT,DELETE),大小写不敏感。
        /// </summary>
        public string Method 
        { 
            get
            {
                if (string.IsNullOrEmpty(m_Method))
                {
                    Debug.LogError($"{Url}没有设置请求类型，这通常是一个错误，系统默认当作GET处理");
                    return "GET";
                }

                return m_Method;
            }

            set
            {
                if(string.IsNullOrEmpty(value))
                {
                    Debug.LogError($"不支持HTTP请求方法:{value},已自动设置为GET");
                    value = "GET";
                }

                switch(value.ToUpper())
                {
                    case "GET":
                    case "POST":
                    case "PUT":
                    case "DELETE":
                        break;

                    default:
                        Debug.LogError($"暂不支持HTTP请求方法:{value}");
                        return;
                }

                m_Method = value.ToUpper();
            }
        }
        protected string ContentType { get; set; } = "application/json";
        protected string Url { get; set; }
        protected byte[] Content { get; set; }
        // 服务器可通过该字段识别客户端类型
        private static string UserAgent = $"UnityCoreFetch-{Application.platform}-{Application.version}";
        protected System.Lazy<List<KeyValuePair<string, string>>> CustomHeaders { get; } = new();
        protected CancellationToken? CancellationToken { get; set; }

        protected HttpRequestBuild()
        {

        }

        protected HttpRequestBuild(string url) : this()
        {
            Url = url;
        }

        public HttpRequestBuild(string url, string method) : this()
        {
            Url = url;
            Method = method;
        }

        public string GetRequestName()
        {
            return Url;
        }

        /// <summary>
        /// 设置生命周期
        /// </summary>
        /// <param name="cancellationToken">生命令牌</param>
        public void SetCancellationToken(CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;
        }

        /// <summary>
        /// 构造一个GET请求
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static HttpRequestBuild Get(string url)
        {
            return new HttpRequestBuild(url)
            {
                Method = "GET"
            };
        }

        /// <summary>
        /// 构造一个POST请求
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static HttpRequestBuild Post(string url)
        {
            return new HttpRequestBuild(url)
            {
                Method = "POST"
            };
        }

        public static HttpRequestBuild Put(string url)
        {
            return new HttpRequestBuild(url)
            {
                Method = "PUT"
            };
        }

        public HttpRequestBuild SetContentType(string contentType)
        {
            if(!string.IsNullOrEmpty(contentType))
                this.ContentType = contentType;
            return this;
        }


        public HttpRequestBuild SetUserAgent(string userAgent)
        {
            UserAgent = userAgent;
            return this;
        }

        public HttpRequestBuild SetUrl(string url)
        {
            this.Url = url;
            return this;
        }

        /// <summary>
        /// 设置上传数据
        /// </summary>
        /// <param name="body">内容</param>
        /// <returns></returns>
        public HttpRequestBuild SetBody(string body)
        {
            return SetBody(System.Text.Encoding.UTF8.GetBytes(body));
        }

        /// <summary>
        /// 设置上传数据
        /// </summary>
        /// <param name="body">内容</param>
        /// <returns></returns>
        public HttpRequestBuild SetBody(byte[] body)
        {
            this.Content = body;
            return this;
        }

        /// <summary>
        /// 设置一个自定定标头
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public HttpRequestBuild SetCustomHeader(string name, string value)
        {
            CustomHeaders.Value.Add(new KeyValuePair<string, string>(name, value));
            return this;
        }

        /// <summary>
        /// 构建为Unity原生默认请求
        /// </summary>
        /// <returns></returns>
        public virtual UnityWebRequest Build()
        {
            UnityWebRequest request;

            switch (this.Method)
            {
                case "GET":
                    request = UnityWebRequest.Get(this.Url);
                    break;

                case "POST":
                    request = UnityWebRequest.Post(this.Url, string.Empty);
                    break;

                case "PUT":
                    request = UnityWebRequest.Put(this.Url, string.Empty);
                    break;

                case "DELETE":
                    request = UnityWebRequest.Delete(this.Url);
                    break;

                default:
                    throw new System.InvalidOperationException($"不支持的请求类型：{this.Method}");
            }

            request.SetRequestHeader("User-Agent", UserAgent);

            // POST和PUT需要上传内容(可能),如果用户没有指定Content-Type则由系统自动指定
            switch (Method)
            {
                case "POST":
                case "PUT":
                    if (string.IsNullOrEmpty(ContentType))
                    {
                        request.SetRequestHeader("Content-Type", ContentType);
                    }
                    if(Content!= null && Content.Length > 0) {
                        request.uploadHandler = new UploadHandlerRaw(Content);
                    }  
                    break;
            }

            // 若用户设置了Content-Type则会覆盖构造器创建的
            if (CustomHeaders.IsValueCreated)
            {
                foreach (var data in CustomHeaders.Value)
                {
                    request.SetRequestHeader(data.Key, data.Value);
                }
            }

            return request;
        }

        /// <summary>
        /// 实现构造器
        /// </summary>
        /// <returns></returns>
        public UnityWebRequest CreateRequest()
        {
            return this.Build();
        }

        void IRequestFactory.SetRequestNodeData(ref EasyRequest request)
        {
            if(this.CancellationToken.HasValue)
                request.CancellationToken = this.CancellationToken.Value;
        }
    }
}