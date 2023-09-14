using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMOD;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using IOTLib.SaveDB.Interceptor;

namespace IOTLib
{
    /// <summary>
    /// 该HTTP系统不依赖GameObject
    /// </summary>
    [SystemDescribe(Author = "吴", Describe = "基础HTTP模块，每帧最多处理99个链接，否则会异常，不依赖于GameObject推进网络请求", Name = "HTTP System", Version = "0.5")]
    [GameSystem(AlwaysRun = true)]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class HttpSystem : BaseSystem
    {
        private readonly Stack<EasyRequest> requestList = new Stack<EasyRequest>();

        public override void OnCreate()
        {
        }

        /// <summary>
        /// 创建一个请求,会自动压入请求栈中。
        /// </summary>
        /// <param name="requestFactory">构建器</param>
        /// <param name="callBack">收到结果回调</param>
        /// <returns></returns>
        public EasyRequest Create(IRequestFactory requestFactory, System.Action<EasyRequest> callBack)
        {
            UnityWebRequest req = requestFactory.CreateRequest();

            // 包装为EasyRequest
            var easy_req = new EasyRequest()
            {
                id = Random.Range(1, int.MaxValue),
                webRequest = req,
                doneCallBack = callBack,
            };

            // 只有编辑器模型才创建基于DB的拦截器
            if(Application.isEditor)
            {
                if (InterceptorServer.GetInterceptorData(requestFactory.GetRequestName(), req.url, out var interData))
                {
                    var downHandle = new BlockDownloadHandler(interData.block_url, req);

                    downHandle.ReturnBody = interData.return_body;

                    if (!(interData.state_code >= 200 && interData.state_code <= 299))
                    {
                        downHandle.ForceError = true;
                    }

                    req.downloadHandler= downHandle;
                }
            }

            requestFactory.SetRequestNodeData(ref easy_req);

            PushRequest(easy_req);

            // 返回包装结果
            return easy_req;
        }
        
        /// <summary>
        /// 创建一个新的请求
        /// </summary>
        /// <param name="factory">构建器</param>
        /// <param name="callback">结果回调</param>
        /// <returns>请求任务</returns>
        public static EasyRequest New(IRequestFactory factory, System.Action<EasyRequest> callback)
        {
            return SystemManager.GetSystem<HttpSystem>().Create(factory, callback);
        }

        /// <summary>
        /// 压入一个请求，通常会在下一帧执行
        /// </summary>
        /// <param name="request"></param>
        public void PushRequest(EasyRequest request)
        {
            // 一帧最多请多99个
            if (requestList.Count >= 99) throw new System.InvalidOperationException("请求异常，一帧下最多请求99个链接");
            requestList.Push(request);
        }

        public override void OnUpdate()
        {
            while (requestList.Count > 0)
            {
                var cur = requestList.Pop();

                UnityWebRequestAsyncOperation asyncOperator;

                asyncOperator = cur.webRequest.SendWebRequest();

                if (cur.CancellationToken.HasValue)
                {
                    // 生命周期已经结束
                    if(cur.CancellationToken.Value.IsCancellationRequested)
                    {
                        cur.webRequest.Dispose();
                        return;
                    }

                    asyncOperator.WithCancellation(cur.CancellationToken.Value);
                }

                asyncOperator.completed += (state) =>
                {
                    if (cur.CancellationToken.HasValue && cur.CancellationToken.Value.IsCancellationRequested)
                    {
                        return;
                    }
                    else
                    {
                        cur.CancellationToken = null;
                    }

                    if (state.isDone)
                    {    
                        if (cur.webRequest.result != UnityWebRequest.Result.Success)
                        {
                            Debug.LogErrorFormat("请求异常({0}):{1},返回:{2}",
                                cur.webRequest.responseCode,
                                cur.webRequest.url,
                                cur.webRequest.downloadHandler.text
                            );
                        }

                        cur.doneCallBack?.Invoke(cur);
                    }
                    else
                    {
                        cur.doneCallBack?.Invoke(cur);
                        Debug.LogError($"处理错误的请求:{cur.webRequest.url}");
                    }
                };
            }
        }
    }
}