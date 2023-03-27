using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IOTLib;
using UMOD;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public static class UnityExtend
{
    #region ��Ϣϵͳ��չ
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
    /// �����¼���ʵ��������ʹ��ָ��GameObject�ı���汾���и��õ�Ч�ʡ�
    /// </summary>
    /// <typeparam name="T">�¼�����</typeparam>
    /// <param name="ievent">�¼�ʵ��</param>
    public static void SendEvent<T>(this MonoBehaviour _, System.Action<T> ievent) where T : IMonoEventHandler
    {
        var mark = UnityEngine.Profiling.CustomSampler.Create("�¼�����");
        mark.Begin();
        var result = GameObject.FindObjectsOfType<DataBehaviour>();
        foreach (var a in result)
        {
            SendEvent<T>(_, a.gameObject, ievent);
        }
        mark.End();
    }

    /// <summary>
    /// �����¼���ʵ��������ʹ��ָ��GameObject�ı���汾���и��õ�Ч�ʡ�
    /// </summary>
    /// <typeparam name="T">�¼�����</typeparam>
    /// <param name="eventData">�¼�����</param>
    /// <param name="ievent">�¼�ʵ��</param>
    public static void SendEvent<T>(this MonoBehaviour _, BaseMonoEventData eventData, System.Action<T, BaseMonoEventData> ievent) where T : IMonoEventHandler
    {
        var mark = UnityEngine.Profiling.CustomSampler.Create("�¼�����");
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
    /// ��ȡ���ߴ���Ŀ�����
    /// </summary>
    /// <typeparam name="T">Ŀ�����</typeparam>
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
    /// ���������оɵı�ǩ�����Ҫ��Ӷ�����ǩ��Ŀ����ʹ��Add����
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
    /// ׷��TAG
    /// </summary>
    /// <param name="target"></param>
    /// <param name="childTag"></param>
    public static void AddTag(this GameObject target, params string[] childTag)
    {
        var dgt = target.GetOrCreateCompoent<DynGameTagAgent>();
        dgt.AddTagRange(childTag);
    }

    /// <summary>
    /// �Ƴ�����Tag 
    /// </summary>
    /// <param name="target"></param>
    /// <param name="rUnityTag">�Ƿ����GameObject.tag?</param>
    /// <param name="tags">����Ķ�����ǩ</param>
    public static void RemoveTag(this GameObject target, params string[] tags)
    {
        var dgt = target.GetOrCreateCompoent<DynGameTagAgent>();
        
        foreach(var a in tags)
        {
            dgt.RemoveTag(a);
        }
    }

    /// <summary>
    /// ���Ŀ���Ƿ����ָ���Ķ�����ǩ��Unityԭ����TagҲ�������������
    /// </summary>
    /// <param name="target"></param>
    /// <param name="any">�������м��ɹ�</param>
    /// <param name="tags">��ǩ��</param>
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
