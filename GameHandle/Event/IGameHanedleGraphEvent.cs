using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using IOTLib;

/// <summary>
/// 一个属于图的事件，不属于内部的子单元
/// </summary>
public interface IGameHanedleGraphEvent : IEvent, IGameHandleEvent
{

    internal UniTask CallFromGameHandleLoop(IFlow flow);

    internal bool Execute(IFlow flow);
}
