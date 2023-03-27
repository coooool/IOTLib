using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace IOTLib
{
    /// <summary>
    /// �ȴ�һ��״̬������OK��������
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