using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// ��������״̬֮��ı任
/// </summary>
public interface IFlowStateTransition : IStateEventListener
{
    public IFlowState SourceState { get;}

    public IFlowState DestinationState { get; }

    /// <summary>
    /// �����˳������ֽ���
    /// </summary>
    bool AllowExitSelf { get; set; }

    /// <summary>
    /// �����������
    /// </summary>
    /// <param name="flow"></param>
    /// <returns></returns>
    UniTask<bool> Execute(Flow flow);

    // �л���֧
    internal UniTask Branch(Flow flow);
}
