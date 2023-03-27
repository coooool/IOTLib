using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// ��С����״̬�ӿ�
/// </summary>
public interface IState
{
    /// <summary>
    /// ����ģʽ
    /// </summary>
    /// <param name="flow">������</param>
    UniTask Enter(IFlow flow);

    /// <summary>
    /// �뿪һ��ģʽ
    /// </summary>
    /// <param name="flow">������</param>
    UniTask Exit(IFlow flow);

    /// <summary>
    /// ÿ֡ˢ��һ����
    /// </summary>
    /// <param name="flow"></param>
    /// <returns></returns>
    UniTask Update(IFlow flow);
}
