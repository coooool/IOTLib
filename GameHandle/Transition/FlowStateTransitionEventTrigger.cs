using Cysharp.Threading.Tasks;
using IOTLib;
using System;
using System.Threading;
using UnityEngine;

public class FlowStateTransitionEventTrigger : FlowStateTransition, ITransitionEvent
{
    public string EventName {get; private set;}

    private bool IsTrigger { get; set; } = false;

    bool IGameHandleEvent.IsListener {get; set;}

    KeyValuePairs? cacheLastEventData;

    public FlowStateTransitionEventTrigger(IFlowState source, IFlowState target, string listenEventName) : base(source, target)
    {
        EventName = listenEventName;
    }

    public void Trigger(KeyValuePairs? eventData)
    {
        if (IsTrigger)
        {
            UnityEngine.Debug.LogWarning($"ͬһ֡�����϶�δ���[{EventName}]�л���֧ʱ��ʹ�����ĵ��¼�����");
        }

        IsTrigger = true;

        cacheLastEventData = eventData;
    }

    private void Reset()
    {
        IsTrigger = false;
        cacheLastEventData?.Clear();
        cacheLastEventData = null;
    }

    protected override void OnInitialization(IFlow flow)
    {
        IsTrigger = false;
        (this as ITransitionEvent).IsListener = true;
    }

    protected override void OnUnitialization(IFlow flow)
    {
        (this as ITransitionEvent).IsListener = false;
    }

    public override UniTask<bool> Execute(Flow flow)
    {
        var result = IsTrigger;

        if(result && cacheLastEventData.HasValue)
        {
            while (cacheLastEventData.Value.Count > 0)
            {
                var arg = cacheLastEventData.Value.Pop();
                flow.Vars.Set(arg.Key, arg.Value);
            }
            cacheLastEventData = null;
        }

        return UniTask.FromResult(result);
    }

    protected async override UniTask Branch(Flow flow)
    {
        try
        {
            await base.Branch(flow);
        }
        catch(OperationCanceledException)
        {

        }
        catch(System.Exception e)
        {
            Debug.LogWarning("�л���֧ʧ��");
            Reset();
            throw e;
        }
        finally
        {
            Reset();
        }       
    }
}
