using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IOTLib;
using UMOD;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public static class UnityExtend
{
    #region 消息系统扩展
    private static MessengerAgent GetMessageAgent(MonoBehaviour mono)
    {
        var agent = mono.GetComponent<MessengerAgent>();
        if (agent == null)
        {
            agent = mono.gameObject.AddComponent<MessengerAgent>();
            agent.hideFlags = HideFlags.HideInInspector;
        }

        return agent;
    }

    // MonoBehaviour
    public static void AddListener(this MonoBehaviour behaviour, string name, Callback func)
    {
        var agent = GetMessageAgent(behaviour);
        agent.AddEventName(name, IOTLib.Messenger.AddListener(name, func));
    }
    public static void AddListener<T>(this MonoBehaviour behaviour, string name, Callback<T> func)
    {
        var agent = GetMessageAgent(behaviour);
        agent.AddEventName(name, Messenger<T>.AddListener(name, func));   
    }
    public static void AddListener<T,U>(this MonoBehaviour behaviour, string name, Callback<T,U> func)
    {
        var agent = GetMessageAgent(behaviour);
        agent.AddEventName(name, Messenger<T, U>.AddListener(name, func));    
    }

    public static void RemoveListener(this MonoBehaviour behaviour, string name)
    {
        var agent = GetMessageAgent(behaviour);
        agent.RemoveEventName(name);
    }

    public static void SendEvent(this MonoBehaviour _, string eventType)
    {
        Messenger.Invoke(eventType);
    }

    public static void SendEvent<T>(this MonoBehaviour _, string name, T arg1)
    {
        Messenger<T>.Invoke(name,arg1);
    }

    public static void SendEvent<T,U>(this MonoBehaviour _, string name, T arg1, U arg2)
    {
        Messenger<T,U>.Invoke(name, arg1, arg2);
    }

    public static void SendEvent<T>(this MonoBehaviour _, GameObject target, System.Action<T> iscript) where T : IMonoEventHandler
    {
        MonoEventExecute.Execute<T>(target, null!, (a, _) => iscript?.Invoke(a));
    }

    public static void SendEvent<T>(this MonoBehaviour _, GameObject target, BaseMonoEventData eventData, System.Action<T, BaseMonoEventData> iscript) where T : IMonoEventHandler
    {
        MonoEventExecute.Execute<T>(target, eventData, (a, b) => iscript?.Invoke(a, b));
    }

    /// <summary>
    /// 发送事件到实例，建议使用指定GameObject的变体版本具有更好的效率。
    /// </summary>
    /// <typeparam name="T">事件类型</typeparam>
    /// <param name="ievent">事件实例</param>
    public static void SendEvent<T>(this MonoBehaviour _, System.Action<T> ievent) where T : IMonoEventHandler
    {
        var mark = UnityEngine.Profiling.CustomSampler.Create("事件触发");
        mark.Begin();
        var result = GameObject.FindObjectsOfType<DataBehaviour>();
        foreach (var a in result)
        {
            SendEvent<T>(_, a.gameObject, ievent);
        }
        mark.End();
    }

    /// <summary>
    /// 发送事件到实例，建议使用指定GameObject的变体版本具有更好的效率。
    /// </summary>
    /// <typeparam name="T">事件类型</typeparam>
    /// <param name="eventData">事件数据</param>
    /// <param name="ievent">事件实例</param>
    public static void SendEvent<T>(this MonoBehaviour _, BaseMonoEventData eventData, System.Action<T, BaseMonoEventData> ievent) where T : IMonoEventHandler
    {
        var mark = UnityEngine.Profiling.CustomSampler.Create("事件触发");
        mark.Begin();
        var result = GameObject.FindObjectsOfType<DataBehaviour>();
        foreach (var a in result)
        {
            SendEvent<T>(_, a.gameObject, eventData, ievent);
        }
        mark.End();
    }
    // ------------------------------------------------

    // BaseSystem -------------------------------------
    public static void SendEvent(this BaseSystem _, string eventType)
    {
        Messenger.Invoke(eventType);
    }

    public static void SendEvent<T>(this BaseSystem _, string name, T arg1)
    {
        Messenger<T>.Invoke(name, arg1);
    }

    public static void SendEvent<T>(this BaseSystem _, GameObject target, System.Action<T> iscript) where T : IMonoEventHandler
    {
        MonoEventExecute.Execute<T>(target, null!, (a, _) => iscript?.Invoke(a));
    }

    public static void SendEvent<T>(this BaseSystem _, GameObject target, BaseMonoEventData eventData, System.Action<T, BaseMonoEventData> iscript) where T : IMonoEventHandler
    {
        MonoEventExecute.Execute<T>(target, eventData, (a, b) => iscript?.Invoke(a, b));
    }

    public static void SendEvent<T, U>(this BaseSystem _, string name, T arg1, U arg2)
    {
        Messenger<T, U>.Invoke(name, arg1, arg2);
    }
    // ------------------------------------------------

    #endregion

    #region UMOD
    public static T? GetSystem<T>(this MonoBehaviour target) where T: BaseSystem
    {
        return GetSystem(target, typeof(T)) as T;
    }

    public static IBaseSystem GetSystem(this MonoBehaviour _, System.Type type)
    {
        return SystemManager.GetSystem(type);
    }
    #endregion


    #region GameHandle
    public static UniTask SetSafeDestory(this Tween tween, IFlowStateGraph graph)
    {
        DoTweenObjectDestroy.ProcessLoopTween(tween, graph.Component);
        return tween.WithCancellation(graph.GraphDestroyCancelToken);
    }
    public static UniTask SetSafeDestory(this Tween tween, FlowState state)
    {
        DoTweenObjectDestroy.ProcessLoopTween(tween, state.transform);
        return tween.WithCancellation(state.DestroyOrExitStateCancelToken);
    }
    public static UniTask SetSafeDestory(this Tween tween, Component mono)
    {
        DoTweenObjectDestroy.ProcessLoopTween(tween, mono);
        return tween.WithCancellation(mono.gameObject.GetCancellationTokenOnDestroy());
    }
    public static UniTask SetSafeDestory(this Tweener tween, Component mono)
    {
        DoTweenObjectDestroy.ProcessLoopTween(tween, mono);
        return tween.WithCancellation(mono.gameObject.GetCancellationTokenOnDestroy());
    }
    #endregion

    #region Component
    /// <summary>
    /// 获取或者创建目标组件
    /// </summary>
    /// <typeparam name="T">目标组件</typeparam>
    /// <param name="target"></param>
    /// <returns></returns>
    public static T GetOrCreateCompoent<T>(this Component target) where T:Component
    {
        return GetOrCreateCompoent<T>(target.gameObject);
    }
    public static T GetOrCreateCompoent<T>(this GameObject target) where T : Component
    {
        if(target.TryGetComponent<T>(out var c))
        {
            return c;
        }

        return target.AddComponent<T>();
    }
    #endregion

    #region GameObjectTags
    /// <summary>
    /// 这会清除所有旧的标签，如果要添加二级标签到目标请使用Add方法
    /// </summary>
    /// <param name="target"></param>
    /// <param name="unityTag"></param>
    /// <param name="childTag"></param>
    public static void SetTag(this GameObject target, params string[] childTag)
    {
        var dgt = target.GetOrCreateCompoent<DynGameTagAgent>();
        dgt.SetTag(childTag);
    }

    /// <summary>
    /// 追加TAG
    /// </summary>
    /// <param name="target"></param>
    /// <param name="childTag"></param>
    public static void AddTag(this GameObject target, params string[] childTag)
    {
        var dgt = target.GetOrCreateCompoent<DynGameTagAgent>();
        dgt.AddTagRange(childTag);
    }

    /// <summary>
    /// 移除二级Tag 
    /// </summary>
    /// <param name="target"></param>
    /// <param name="rUnityTag">是否清除GameObject.tag?</param>
    /// <param name="tags">清理的二级标签</param>
    public static void RemoveTag(this GameObject target, params string[] tags)
    {
        var dgt = target.GetOrCreateCompoent<DynGameTagAgent>();
        
        foreach(var a in tags)
        {
            dgt.RemoveTag(a);
        }
    }

    /// <summary>
    /// 检测目标是否存在指定的二级标签，Unity原生的Tag也包含在这里测试
    /// </summary>
    /// <param name="target"></param>
    /// <param name="any">任意命中即成功</param>
    /// <param name="tags">标签组</param>
    public static bool HasTag(this GameObject target, bool any, params string[] tags)
    {
        var dgt = target.GetOrCreateCompoent<DynGameTagAgent>();
        var result = dgt.CompareTag(any, tags);

        if(false == result)
        {
            foreach(var a in tags)
            {
                if (target.tag == a) 
                    return true;
            }
        }

        return result;
    }
    #endregion
}
