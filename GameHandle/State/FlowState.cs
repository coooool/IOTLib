using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// 一个基于流的基础状态
/// </summary>
public class FlowState : BaseFlowState
{
    public FlowState(string stateName = null, bool isStart = false) : base(stateName, isStart)
    {

    }
}
