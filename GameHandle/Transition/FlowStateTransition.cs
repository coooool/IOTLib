using Cysharp.Threading.Tasks;
using System;
using UnityEngine;


/// <summary>
/// 状态流变换器
/// StartListener和StopListener是同步的，进入或离开会立即创建或销毁事件侦听
/// </summary>
public class FlowStateTransition : IFlowStateTransition
{
    public IFlowState SourceState { get; private set; } = null;
    public IFlowState DestinationState { get; private set; } = null;

    public bool IsListener { get; private set; } = false;

    public bool AllowExitSelf { get; set; } = true;

    public FlowStateTransition(IFlowState source, IFlowState target)
    {
        SourceState = source;
        DestinationState = target;

        SourceState.AllOutTransition.Add(this);
    }

    void IStateEventListener.StartListener(IFlow flow)
    {
        IsListener = true;
        OnInitialization(flow);
    }

    void IStateEventListener.StopListener(IFlow flow)
    {
        IsListener = false;
        OnUnitialization(flow);
    }

    /// <summary>
    /// 开放的初始化，用户在这里实现事件侦听
    /// </summary>
    /// <param name="flow"></param>
    protected virtual void OnInitialization(IFlow flow) { }

    /// <summary>
    /// 用户在这里实现事件释放
    /// </summary>
    /// <param name="flow"></param>
    protected virtual void OnUnitialization(IFlow flow) { }

    /// <summary>
    /// 变换在返回True时触发分支变换
    /// </summary>
    /// <param name="flow"></param>
    /// <returns></returns>
    public virtual UniTask<bool> Execute(Flow flow)
    {
        return UniTask.FromResult(false);
    }

    protected async virtual UniTask Branch(Flow flow)
    {
        //flow.PushTransitionHistory(this);

        if(!AllowExitSelf && DestinationState != null && DestinationState.IsListener)
        {
            Debug.LogError($"切换分支失败:[{DestinationState.StateName}]已经是激活的");
            throw new OperationCanceledException();
        }

        if (SourceState == null)
        {
            Debug.LogError($"退出源分支失败,来源为NULL");
        }
        else
        {
            if(SourceState is AnyState ass)
            {
                if (ass.TryGetRootState(ass, out var root)) 
                    await root.ExitAndStopListener(flow);
            }
            else
                await SourceState.ExitAndStopListener(flow);
        }

        if (DestinationState != null)
        {
            if(DestinationState is AnyState ass)
            {
                if (ass.TryGetRootState(ass, out var root))
                    await root.EnterAndListener(flow);
            }
            else await DestinationState.EnterAndListener(flow);
        }
        else
        {
            Debug.LogError($"进入目标分支失败,目标为NULL");
        }
    }

    async UniTask IFlowStateTransition.Branch(Flow flow)
    {
        await Branch(flow);
    }
}
