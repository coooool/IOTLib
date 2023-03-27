using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public static class FlowStateUtility 
{
    public async static UniTask EnterAndListener(this IFlowState state, IFlow flow)
    {
        if (state.IsListener == false)
        {
            state.StartListener(flow);
            await state.Enter(flow);
        }
    }

    public async static UniTask ExitAndStopListener(this IFlowState state, IFlow flow)
    {
        if (state.IsListener)
        {
            state.StopListener(flow);
            await state.Exit(flow);
        }
    }
}
