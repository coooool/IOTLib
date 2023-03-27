using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using IOTLib;

/// <summary>
/// һ������ͼ���¼����������ڲ����ӵ�Ԫ
/// </summary>
public interface IGameHanedleGraphEvent : IEvent, IGameHandleEvent
{

    internal UniTask CallFromGameHandleLoop(IFlow flow);

    internal bool Execute(IFlow flow);
}
