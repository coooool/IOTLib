using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UMOD;
using UnityEngine;
using IOTLib;
using IOTLib.IUISystem;

public static class HttpSystemExtend
{
    #region MonoBehavior
    /// <summary>
    /// 创建一个安全的Http请求
    /// </summary>
    /// <param name="mono"></param>
    /// <param name="config_request_name">接口名称</param>
    public static EasyRequest CreateHttpRequest(
        this MonoBehaviour mono, 
        string config_request_name, 
        Action<EasyRequest> callBack)
    {
        var cancelToken = mono.gameObject.GetCancellationTokenOnDestroy();

        if(mono.GetCustomCancelToken(out var t))
        {
            cancelToken = t;
        }

        return HttpSystem.New(
            new ConfigHttpBuild(config_request_name, cancelToken),
            callBack
        );
    }

    /// <summary>
    /// 创建一个安全的HTTP请求
    /// </summary>
    /// <param name="mono"></param>
    /// <param name="config_request_name">接口名称</param>
    /// <param name="parameter">请求参数</param>
    /// <param name="callBack">完成回调</param>
    public static EasyRequest CreateHttpRequest(
        this MonoBehaviour mono,
        string config_request_name,
        System.Object parameter,
        Action<EasyRequest> callBack)
    {
        var cancelToken = mono.gameObject.GetCancellationTokenOnDestroy();

        if (mono.GetCustomCancelToken(out var t))
        {
            cancelToken = t;
        }

        return HttpSystem.New(
            new ConfigHttpBuild(config_request_name, cancelToken).SetParameter(parameter),
            callBack
        );
    }

    public static EasyRequest CreateHttpRequest(
        this MonoBehaviour mono,
        string config_request_name,
        KeyValuePairs parameter,
        Action<EasyRequest> callBack)
    {
        var cancelToken = mono.gameObject.GetCancellationTokenOnDestroy();

        if (mono.GetCustomCancelToken(out var t))
        {
            cancelToken = t;
        }

        return HttpSystem.New(
            new ConfigHttpBuild(config_request_name, cancelToken).SetParameter(parameter),
            callBack
        );
    }

    /// <summary>
    /// 创建一个安全的HTTP请求
    /// </summary>
    /// <param name="mono"></param>
    /// <param name="iBuild">构建器</param>
    /// <param name="callBack"></param>
    /// <returns></returns>
    public static EasyRequest CreateHttpRequest(
        this MonoBehaviour mono,
        IRequestFactory iBuild,
        Action<EasyRequest> callBack)
    {
        var cancelToken = mono.gameObject.GetCancellationTokenOnDestroy();

        if (mono.GetCustomCancelToken(out var t))
        {
            cancelToken = t;
        }

        iBuild.SetCancellationToken(cancelToken);

        return HttpSystem.New(iBuild, callBack);
    }
    #endregion

    #region BaseFlowState
    /// <summary>
    /// 创建一个安全的HTTP请求
    /// </summary>
    /// <param name="mono"></param>
    /// <param name="config_request_name">请求名称</param>
    /// <param name="callBack">成功回调</param>
    public static EasyRequest CreateHttpRequest(
        this BaseFlowState mono,
        string config_request_name,
        Action<EasyRequest> callBack)
    {
        return HttpSystem.New(
            new ConfigHttpBuild(config_request_name, mono.DestroyOrExitStateCancelToken),
            callBack
        );
    }
    public static EasyRequest CreateHttpRequest(
        this BaseFlowState mono,
        string config_request_name,
        object parameter,
        Action<EasyRequest> callBack)
    {
        return HttpSystem.New(
            new ConfigHttpBuild(config_request_name, mono.DestroyOrExitStateCancelToken).SetParameter(parameter),
            callBack
        );
    }
    public static EasyRequest CreateHttpRequest(
        this BaseFlowState mono,
        string config_request_name,
        KeyValuePairs parameter,
        Action<EasyRequest> callBack)
    {
        return HttpSystem.New(
            new ConfigHttpBuild(config_request_name, mono.DestroyOrExitStateCancelToken).SetParameter(parameter),
            callBack
        );
    }

    public static EasyRequest CreateHttpRequest(
        this BaseFlowState state,
        IRequestFactory iBuild,
        Action<EasyRequest> callBack)
    {
        var cancelToken = state.DestroyOrExitStateCancelToken;

        iBuild.SetCancellationToken(cancelToken);

        return HttpSystem.New(iBuild, callBack);
    }
    #endregion

    #region BaseSystem
    public static EasyRequest CreateHttpRequest(
        this BaseSystem sys,
        string config_request_name,
        Action<EasyRequest> callBack)
    {
        return HttpSystem.New(
            new ConfigHttpBuild(config_request_name, sys.GetDropCancellationToken()),
            callBack
        );
    }
    public static EasyRequest CreateHttpRequest(
        this BaseSystem sys,
        string config_request_name,
        object parameter,
        Action<EasyRequest> callBack)
    {
        return HttpSystem.New(
            new ConfigHttpBuild(config_request_name, sys.GetDropCancellationToken()).SetParameter(parameter),
            callBack
        );
    }
    public static EasyRequest CreateHttpRequest(
        this BaseSystem sys,
        string config_request_name,
        KeyValuePairs parameter,
        Action<EasyRequest> callBack)
    {
        return HttpSystem.New(
            new ConfigHttpBuild(config_request_name, sys.GetDropCancellationToken()).SetParameter(parameter),
            callBack
        );
    }

    public static EasyRequest CreateHttpRequest(
       this BaseSystem sys,
       IRequestFactory iBuild,
       Action<EasyRequest> callBack)
    {
        var cancelToken = sys.GetDropCancellationToken();

        iBuild.SetCancellationToken(cancelToken);

        return HttpSystem.New(iBuild, callBack);
    }
    #endregion
}
