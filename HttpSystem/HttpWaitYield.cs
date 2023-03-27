using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace IOTLib
{
    /// <summary>
    /// 等待一个状态，调用OK不再阻塞
    /// </summary>
    public class HttpWaitYield : CustomYieldInstruction
    {
        private readonly WeakReference<UnityWebRequest> request;

        public HttpWaitYield(UnityWebRequest webRequest)
        {
            request= new WeakReference<UnityWebRequest>(webRequest);
        }

        public override bool keepWaiting
        {
            get
            {
                if(request.TryGetTarget(out UnityWebRequest webRequest))
                {
                    return !webRequest.isDone;
                }

                return false;
            }
        }
    }
}