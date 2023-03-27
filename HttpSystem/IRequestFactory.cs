using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace IOTLib
{
    /// <summary>
    /// IP构造接口
    /// </summary>
    public interface IRequestFactory
    {
        /// <summary>
        /// 获取请求名
        /// </summary>
        /// <returns></returns>
        string GetRequestName();
        /// <summary>
        /// 创建一个请求
        /// </summary>
        /// <returns></returns>
        UnityWebRequest CreateRequest();

        /// <summary>
        /// 进入请求前会回调一次允许构建器修改一些数据
        /// </summary>
        /// <param name="request"></param>
        internal void SetRequestNodeData(ref EasyRequest request);
    }
}
