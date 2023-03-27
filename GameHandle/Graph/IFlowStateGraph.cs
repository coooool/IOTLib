using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public interface IFlowStateGraph : IStateGraph, IEnumerable<IFlowState>
{
    List<IFlowState> Units { get; }

    // �������ڿ���
    CancellationToken GraphDestroyCancelToken { get; }

    string GUID { get; }

    /// <summary>
    /// ��ʼ����
    /// </summary>
    /// <param name="flow"></param>
    internal UniTask StartListener(IFlow flow);

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="flow"></param>
    internal UniTask StopListener(IFlow flow);

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Units.GetEnumerator();
    }

    IEnumerator<IFlowState> IEnumerable<IFlowState>.GetEnumerator()
    {
        return Units.GetEnumerator();
    }

    T GetUnit<T>() where T : BaseFlowState;

    Behaviour Component { get; set; }
}
