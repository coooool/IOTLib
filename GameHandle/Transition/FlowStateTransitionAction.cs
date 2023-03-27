using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// 这是一个可快速创建的变换器
/// </summary>
public class FlowStateTransitionAction : FlowStateTransition
{
    private System.Func<Flow, System.Threading.CancellationToken, UniTask<bool>> ExecuteEvent;

    private System.Threading.CancellationToken cancellationToken;

    public FlowStateTransitionAction(IFlowState source, IFlowState target, System.Threading.CancellationToken graphLiftToken, System.Func<Flow, System.Threading.CancellationToken,UniTask<bool>> ExecuteAction) : base(source, target)
    {
        cancellationToken = graphLiftToken;
        ExecuteEvent = ExecuteAction;
    }

    public void SetCustomExecuteEvent(System.Func<Flow, System.Threading.CancellationToken, UniTask<bool>> ExecuteAction)
    {
        ExecuteEvent = ExecuteAction;
    }

    public override async UniTask<bool> Execute(Flow flow)
    {
        if(ExecuteEvent != null)
        {
            var result = await ExecuteEvent.Invoke(flow, cancellationToken);
            return result;
        }

        return false;
    }
}
