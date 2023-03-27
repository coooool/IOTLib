using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace IOTLib
{
    /// <summary>
    /// HTTP �������ݹ�����
    /// ʵ�ʷ���������HTTPSystem�ƽ���������ֻ����һ���������������
    /// GET��POST��Ĭ����������ݱ���Ϊapplication/json
    /// ��Ȩ��ͷ���û�ϵͳע����HTTPSYSTEM��������������ṩ
    /// </summary>
    public class HttpRequestBuild : IRequestFactory
    {
        // Ϊ��ʱĬ��GET
        private string m_Method;

        /// <summary>
        /// HTTP��������(GET,POST,PUT,DELETE),��Сд�����С�
        /// </summary>
        public string Method 
        { 
            get
            {
                if (string.IsNullOrEmpty(m_Method))
                {
                    Debug.LogError($"{Url}û�������������ͣ���ͨ����һ������ϵͳĬ�ϵ���GET����");
                    return "GET";
                }

                return m_Method;
            }

            set
            {
                if(string.IsNullOrEmpty(value))
                {
                    Debug.LogError($"��֧��HTTP���󷽷�:{value},���Զ�����ΪGET");
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
                        Debug.LogError($"�ݲ�֧��HTTP���󷽷�:{value}");
                        return;
                }

                m_Method = value.ToUpper();
            }
        }
        protected string ContentType { get; set; } = "application/json";
        protected string Url { get; set; }
        protected byte[] Content { get; set; }
        // ��������ͨ�����ֶ�ʶ��ͻ�������
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
        /// ������������
        /// </summary>
        /// <param name="cancellationToken">��������</param>
        public void SetCancellationToken(CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;
        }

        /// <summary>
        /// ����һ��GET����
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
        /// ����һ��POST����
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
        /// �����ϴ�����
        /// </summary>
        /// <param name="body">����</param>
        /// <returns></returns>
        public HttpRequestBuild SetBody(string body)
        {
            return SetBody(System.Text.Encoding.UTF8.GetBytes(body));
        }

        /// <summary>
        /// �����ϴ�����
        /// </summary>
        /// <param name="body">����</param>
        /// <returns></returns>
        public HttpRequestBuild SetBody(byte[] body)
        {
            this.Content = body;
            return this;
        }

        /// <summary>
        /// ����һ���Զ�����ͷ
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
        /// ����ΪUnityԭ��Ĭ������
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
                    throw new System.InvalidOperationException($"��֧�ֵ��������ͣ�{this.Method}");
            }

            request.SetRequestHeader("User-Agent", UserAgent);

            // POST��PUT��Ҫ�ϴ�����(����),����û�û��ָ��Content-Type����ϵͳ�Զ�ָ��
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

            // ���û�������Content-Type��Ḳ�ǹ�����������
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
        /// ʵ�ֹ�����
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