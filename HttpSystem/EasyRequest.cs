using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace IOTLib
{
    /// <summary>
    /// 请求数据包
    /// </summary>
    public struct EasyRequest
    {
        internal System.Action<EasyRequest> doneCallBack;
        internal CancellationToken? CancellationToken { get; set; }

        public int id;
        public UnityWebRequest webRequest;

        public string ResponseText
        {
            get
            {
                if(webRequest.downloadedBytes == 0 && webRequest.downloadHandler == null)
                {
                    Debug.LogWarning($"接口返回空数据:{webRequest.url}");
                    return string.Empty;
                }

                return webRequest.downloadHandler.text;
            }
        }

        public bool Success => webRequest != null && webRequest.result == UnityWebRequest.Result.Success;

        /// <summary>
        /// 获取纹理
        /// </summary>
        public Texture2D Texture2D => DownloadHandlerTexture.GetContent(this.webRequest);

        /// <summary>
        /// 获取Unity原生协程版的等侍
        /// </summary>
        /// <returns></returns>
        public HttpWaitYield ToAsyncUnityIEnumerator()
        {
            return new HttpWaitYield(webRequest);
        }

        /// <summary>
        /// 获取异步版
        /// </summary>
        /// <returns></returns>
        public UniTask ToAsync()
        {
            if(CancellationToken.HasValue)
            {
                return new HttpWaitYield(webRequest).WithCancellation(CancellationToken.Value);
            }

            return new HttpWaitYield(webRequest).ToUniTask();
        }
    }
}