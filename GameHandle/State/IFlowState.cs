using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using IOTLib;

/// <summary>
/// 所有子状态之间必须完全独立设计，不能依赖上一个或下一个状态。
/// </summary>
public interface IFlowState : IState, IStateEventListener
{
    /// <summary>
    /// 状态名称， 可以是匿名的, 初始化后无法再次修改
    /// </summary>
    string StateName { get; }

    /// <summary>
    /// 状态ID不能为空
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
