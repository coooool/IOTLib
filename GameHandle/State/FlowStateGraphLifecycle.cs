using UnityEngine;
using UMOD;
using System;
using System.Threading;
using System.Linq;
using IOTLib;

/// <summary>
/// 流程图的生命周期
/// </summary>
[BindSystem(typeof(GameHandleSystem))]
[DisallowMultipleComponent]
public sealed class FlowStateGraphLifecycle : DataBehaviour, IGraphEventListen
{
    public FlowStateGraph GraphRef { get; private set; } = null;

    CancellationTokenSource cancellationTokenSource;

    bool InitCalled = false;

    public CancellationToken CancellationToken
    {
        get
        {
            if (cancellationTokenSource == null)
            {
                cancellationTokenSource = new CancellationTokenSource();
            }

            return cancellationTokenSource.Token;
        }
    }

    /// <summary>
    /// 由作业系统初始化一次
    /// </summary>
    /// <param name="flowGraph"></param>
    /// <exception cref="InvalidOperationException"></exception>
    internal void Init(IFlowStateGraph flowGraph)
    {
        this.hideFlags = HideFlags.HideInHierarchy;

        if (flowGraph is FlowStateGraph fsg)
        {
            GraphRef = fsg;
            GraphRef.Component = this;
            fsg.GraphDestroyCancelToken = CancellationToken;
        }
        else
        {
            throw new InvalidOperationException("无效的图引用");
        }

        InitCalled = true;
    }

    private void OnEnable()
    {
        if (InitCalled && TryGetTarget(out var fsg))
        {
            fsg.GraphDestroyCancelToken = CancellationToken;
        }
    }

    private void OnDisable()
    {
        if (InitCalled && TryGetTarget(out var _))
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = null;
        }
    }

    protected override void OnDrop()
    {
        OnDisable();
        //Debug.LogFormat("{0},{1}", GraphRef.Name, GraphRef.Component.gameObject.name);
        GraphRef.Component = null;
        GraphRef = null;
    }

    public bool TryGetTarget(out FlowStateGraph value)
    {
        value = null;
        if (GraphRef == null)
        {
            Debug.LogError("图引用已被释放，但还在尝试获取。");
            return false;
        }

        value = GraphRef;

        return true;
    }

    /// <summary>
    /// 测试这个图中的变换事件
    /// </summary>
    /// <param name="name"></param>
    /// <param name="flow"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    bool IGraphEventListen.TestEvent(string name, out IGameHandleEvent hanedleEvent)
    {
        hanedleEvent = null;

        if (TryGetTarget(out var fsg))
        {
            foreach (var state in fsg.Units.Where(p => p.IsListener))
            {
                if (state.TestEventName(name, out hanedleEvent))
                {
                    return true;
                }

                foreach (var trans in state.AllOutTransition)
                {
                    if (trans is ITransitionEvent ige)
                    {
                        if (ige.EventName == name)
                        {
                            hanedleEvent = ige;
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }
}
