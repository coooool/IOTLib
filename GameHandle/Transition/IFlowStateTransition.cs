using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// 连接两个状态之间的变换
/// </summary>
public interface IFlowStateTransition : IStateEventListener
{
    public IFlowState SourceState { get;}

    public IFlowState DestinationState { get; }

    /// <summary>
    /// 允许退出自身又进入
    /// </summary>
    bool AllowExitSelf { get; set; }

    /// <summary>
    /// 触发条件检查
    /// </summary>
    /// <param name="flow"></param>
    /// <returns></returns>
    UniTask<bool> Execute(Flow flow);

    // 切换分支
    internal UniTask Branch(Flow flow);
}
