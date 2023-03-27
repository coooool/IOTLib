using IOTLib.SaveDB.Interceptor;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

namespace IOTLib
{
    /// <summary>
    /// 拦截器下载句柄
    /// </summary>
    public class BlockDownloadHandler : DownloadHandlerScript
    {
        public string BlockUrl { get; set; }

        public string ReturnBody { get; set; }

        // 成功拦截?
        protected bool SuccessBlock { get; set; } = false;

        public bool ForceError { get; set; }

        private WeakReference<UnityWebRequest> WeakReference;

        public BlockDownloadHandler(string interceptUrl, UnityWebRequest req)
        {
            BlockUrl = interceptUrl;    
            WeakReference= new WeakReference<UnityWebRequest>(req);
        }

        protected override string GetText()
        {
            if (SuccessBlock)
            {
                return ReturnBody;
            }

            return base.GetText();
        }

        protected override byte[] GetData()
        {
            if (SuccessBlock)
            {
                return UTF8Encoding.UTF8.GetBytes(ReturnBody);
            }

            return base.GetData();
        }


        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            if (ForceError)
            {
                if (WeakReference.TryGetTarget(out var req))
                {
                    Debug.LogWarning($"网络拦截器对{BlockUrl}进行了枪毙");
                    req.Abort();
                    return false;
                }
            }

            return base.ReceiveData(data, dataLength);
        }

        protected override void CompleteContent()
        {
            SuccessBlock = true;
            Debug.LogWarning($"拦截:{BlockUrl}成功!");
        }
    }
}
