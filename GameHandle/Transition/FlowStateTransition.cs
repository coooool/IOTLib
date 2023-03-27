using Cysharp.Threading.Tasks;
using System;
using UnityEngine;


/// <summary>
/// ״̬���任��
/// StartListener��StopListener��ͬ���ģ�������뿪�����������������¼�����
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
    /// ���ŵĳ�ʼ�����û�������ʵ���¼�����
    /// </summary>
    /// <param name="flow"></param>
    protected virtual void OnInitialization(IFlow flow) { }

    /// <summary>
    /// �û�������ʵ���¼��ͷ�
    /// </summary>
    /// <param name="flow"></param>
    protected virtual void OnUnitialization(IFlow flow) { }

    /// <summary>
    /// �任�ڷ���Trueʱ������֧�任
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
            Debug.LogError($"�л���֧ʧ��:[{DestinationState.StateName}]�Ѿ��Ǽ����");
            throw new OperationCanceledException();
        }

        if (SourceState == null)
        {
            Debug.LogError($"�˳�Դ��֧ʧ��,��ԴΪNULL");
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
            Debug.LogError($"����Ŀ���֧ʧ��,Ŀ��ΪNULL");
        }
    }

    async UniTask IFlowStateTransition.Branch(Flow flow)
    {
        await Branch(flow);
    }
}
