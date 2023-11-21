using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Assertions;
using IOTLib;
using System;

public abstract class BaseFlowState : IFlowState
{
    public string GUID { get; private set; }

    private string m_StateName;

    public string StateName
    {
        get
        {
            if (string.IsNullOrEmpty(m_StateName)) return GUID;
            return m_StateName;
        }
        set
        {
            m_StateName = value;
        }
    }

    /// <summary>
    /// 每入触发事件都会改变ID
    /// Set 0 随意一个数值修改它
    /// </summary>
    private uint m_LastSnaphostId = 0;
    public uint LastSnaphostId
    {
        get
        {  
            return m_LastSnaphostId;
        }

        set
        {
            m_LastSnaphostId = (uint)UnityEngine.Random.Range(1, int.MaxValue);
        }
    }

    public bool IsListener { get; private set; }

    IFlowStateGraph IFlowState.GraphRef { get; set; } = null;

    List<IFlowStateTransition> IFlowState.AllOutTransition { get; set; } = new();

    /// <summary>
    /// 无依赖的状态退出时生命周期,通常在释放场景时会影响这个Token
    /// </summary>
    public CancellationToken LifecycleCancellationToken => GetThisGraphRef()!.GraphDestroyCancelToken;

    CancellationTokenSource IFlowState.StateCancellationToken { get; set; }

    /// <summary>
    /// 有依赖的生命周期
    /// </summary>
    public CancellationToken DestroyOrExitStateCancelToken => CancellationTokenSource.CreateLinkedTokenSource(LifecycleCancellationToken, ((IFlowState)this).StateCancellationToken.Token).Token;

    List<IGameHanedleGraphEvent> IFlowState.EventListenerList { get; set; } = new();

    /// <summary>
    /// 默认启动荐？
    /// </summary>
    public virtual bool IsStart { get; set; }

    #region 分析器
    //#if UNITY_EDITOR
    public bool Anlaysis_FoldoutTransition { set; get; } = true;
    IEnumerable<IFlowStateTransition> IFlowState.AllOutTransitions => (this as IFlowState).AllOutTransition;
    //#endif
    #endregion

    public BaseFlowState(bool isStart)
    {
        GUID = System.Guid.NewGuid().ToString();
        IsStart = isStart;
    }

    public BaseFlowState(string stateName, bool isStart = false) : this(isStart)
    {
        StateName = stateName;
    }

    public IFlowStateGraph GetThisGraphRef()
    {
        var refg = (this as IFlowState).GraphRef;   

        if(refg == null)
            throw new InvalidOperationException($"无法识别的图类型:{this.StateName}可能未被AddState添加");

        return refg;
    }

    protected void AddStateEventListen(string name, System.Func<IFlow, UniTask> action)
    {
        var @this = this as IFlowState;
        @this.EventListenerList.Add(new FlowStateEventUnit(name, action));
    }

    protected void RemoveStateEventListen(string name)
    {
        var @this = this as IFlowState;
        for (var i = 0; i < @this.EventListenerList.Count - 1; i--) {
            var node = @this.EventListenerList[i];
            if(node.EventName == name)
            {
                node.IsListener = false;
                @this.EventListenerList.RemoveAt(i);
            }
        }
    }

    #region 开公事件
    public virtual UniTask Enter(IFlow flow)
    {
        LastSnaphostId = 0;
        return UniTask.CompletedTask;
    }

    public virtual UniTask Exit(IFlow flow)
    {
        return UniTask.CompletedTask;
    }

    public virtual UniTask Update(IFlow flow)
    {
        return UniTask.CompletedTask;
    }
    #endregion

    #region 处理变换节点，激活侦听他们的事件

    void IStateEventListener.StartListener(IFlow flow)
    {
        this.StartListener(flow);
    }

    void IStateEventListener.StopListener(IFlow flow)
    {
        this.StopListener(flow);
    }

    protected virtual void StartListener(IFlow flow)
    {
        if(this.IsListener)
        {
            Debug.LogWarning($"重复侦听");
            return;
        }
        this.IsListener = true;

        var @this = this as IFlowState;

        // 安全原因先检查一次Token，防止任务泄露
        if (@this.StateCancellationToken != null && @this.StateCancellationToken.IsCancellationRequested == false)
        {
            Debug.LogWarning($"{this.StateName}可能发生了泄露事件,请检查代码");

            @this.StateCancellationToken.Cancel();
            @this.StateCancellationToken.Dispose();
        }

        @this.StateCancellationToken = new CancellationTokenSource();

        foreach (var x in @this.AllOutTransition)
        {
            x.StartListener(flow);
        }

        foreach (var x in @this.EventListenerList)
        {
            x.IsListener = true;
        }
    }

    protected virtual void StopListener(IFlow flow)
    {
        if(this.IsListener == false)
        {
            Debug.LogWarning($"停止侦听了一个未激活的状态");
            return;
        }

        this.IsListener = false;

        var @this = this as IFlowState;   
        foreach (var x in @this.AllOutTransition)
        {
            x.StopListener(flow);
        }

        foreach (var x in @this.EventListenerList)
        {
            x.IsListener = false;
        }

        @this.StateCancellationToken.Cancel();
        @this.StateCancellationToken.Dispose();
    }
    #endregion

    #region 事件处理
    bool IFlowState.TestEventName(string name, out IGameHandleEvent hanedleUnityEvent)
    {
        hanedleUnityEvent = null;

        var @this = this as IFlowState;
        foreach(var a in @this.EventListenerList)
        {
            if (a.IsListener && a.EventName == name)
            {
                hanedleUnityEvent = a;
                return true;
            }
        }

        return false;
    }
    #endregion

    #region 获取Unity组件
    public Transform transform => GetThisGraphRef().Component.transform;
    public GameObject gameObject => GetThisGraphRef().Component.gameObject;
    public Component GetComponent<T>() where T : Component
    {
        return GetThisGraphRef().Component.GetComponent<T>();
    }

    public bool TryGetComponent<T>(out T value) where T : Component
    {
        return GetThisGraphRef().Component.TryGetComponent<T>(out value);
    }
    #endregion
}
