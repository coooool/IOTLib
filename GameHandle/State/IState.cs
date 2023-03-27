using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// 最小基础状态接口
/// </summary>
public interface IState
{
    /// <summary>
    /// 进入模式
    /// </summary>
    /// <param name="flow">运行流</param>
    UniTask Enter(IFlow flow);

    /// <summary>
    /// 离开一个模式
    /// </summary>
    /// <param name="flow">运行流</param>
    UniTask Exit(IFlow flow);

    /// <summary>
    /// 每帧刷新一个流
    /// </summary>
    /// <param name="flow"></param>
    /// <returns></returns>
    UniTask Update(IFlow flow);
}
