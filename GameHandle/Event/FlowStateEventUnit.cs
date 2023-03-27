using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using IOTLib;

public class FlowStateEventUnit : IGameHanedleGraphEvent
{
    public string EventName { get; private set; }

    bool IGameHandleEvent.IsListener { get; set; }

    private System.Func<IFlow,UniTask> m_eventCallback;

    KeyValuePairs? cacheLastEventData;

    private bool IsTrigger { get; set; } = false;

    public FlowStateEventUnit(string eventName, System.Func<IFlow, UniTask> action)
    {
        EventName = eventName;
        m_eventCallback = action;
        (this as IGameHandleEvent).IsListener = true;
    }

    public void Trigger(KeyValuePairs? eventData)
    {
        if (IsTrigger)
        {
            Debug.LogWarning($"同一帧调度上多次触发[{EventName}]事件会使用最后的的事件数据");
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

    async UniTask IGameHanedleGraphEvent.CallFromGameHandleLoop(IFlow flow)
    {
        if (IsTrigger && cacheLastEventData.HasValue)
        {
            while (cacheLastEventData.Value.Count > 0)
            {
                var arg = cacheLastEventData.Value.Pop();
                flow.Vars.Set(arg.Key, arg.Value);
            }

            cacheLastEventData = null;
        }

        await m_eventCallback.Invoke(flow);
        IsTrigger = false;

        Reset();
    }

    bool IGameHanedleGraphEvent.Execute(IFlow flow)
    {
        return IsTrigger;
    }
}
