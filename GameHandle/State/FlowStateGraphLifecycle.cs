using UnityEngine;
using UMOD;
using System;
using System.Threading;
using System.Linq;
using IOTLib;

/// <summary>
/// ����ͼ����������
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
    /// ����ҵϵͳ��ʼ��һ��
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
            throw new InvalidOperationException("��Ч��ͼ����");
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
            Debug.LogError("ͼ�����ѱ��ͷţ������ڳ��Ի�ȡ��");
            return false;
        }

        value = GraphRef;

        return true;
    }

    /// <summary>
    /// �������ͼ�еı任�¼�
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
