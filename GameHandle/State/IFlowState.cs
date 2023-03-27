using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using IOTLib;

/// <summary>
/// ������״̬֮�������ȫ������ƣ�����������һ������һ��״̬��
/// </summary>
public interface IFlowState : IState, IStateEventListener
{
    /// <summary>
    /// ״̬���ƣ� ������������, ��ʼ�����޷��ٴ��޸�
    /// </summary>
    string StateName { get; }

    /// <summary>
    /// ״̬ID����Ϊ��
    /// </summary>
    string GUID { get; }

    bool IsStart { get; set; }

    internal List<IFlowStateTransition> AllOutTransition { get; set; }

    internal IFlowStateGraph GraphRef { get; set; }

    CancellationToken LifecycleCancellationToken { get; }

    internal CancellationTokenSource StateCancellationToken { get; set; }

    internal List<IGameHanedleGraphEvent> EventListenerList { get; set; }

    internal bool TestEventName(string name, out IGameHandleEvent hanedleUnityEvent);

    public IEnumerable<IFlowStateTransition> AllOutTransitions { get; }

    //#if UNITY_EDITOR
    public bool Anlaysis_FoldoutTransition { get; set; }  
    //#endif
}
