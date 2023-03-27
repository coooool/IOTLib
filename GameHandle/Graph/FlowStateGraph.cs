using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading;

/// <summary>
/// 一个标准状态图
/// </summary>
public class FlowStateGraph : IFlowStateGraph
{
    GraphTypeEnum IGraph.GraphType => GraphTypeEnum.FlowStateGraph;

    public CancellationToken GraphDestroyCancelToken { get; internal set; }

    public string Name { get; set; } = string.Empty;

    public List<IFlowState> Units { get; private set; } = new();

    public string GUID { get; private set; }

    public Behaviour Component { get; set; } = null;

    public FlowStateGraph(string name)
    {
        Name = name;
        GUID = System.Guid.NewGuid().ToString();
    }

    public bool AddState(IFlowState state)
    {
        if (Units.Contains(state))
        {
            Debug.LogWarning($"正在尝试添加一个相同的单元");
            return false;
        }

        state.GraphRef = this;
        Units.Add(state);

        return true;
    }

    public bool HasState(string name)
    {
        var state = Units.Find(p => p.StateName == name);

        if (null == state)
        {
            Debug.LogError($"找不到状态:{name}");
            return false;
        }

        return RemoveState(state);
    }

    public bool RemoveState(IFlowState state)
    {
        return Units.Remove(state);
    }

    public bool RemoveState(string name)
    {
        var result = false;

        for (var i = Units.Count - 1; i >= 0; i--)
        {
            if (Units[i].StateName == name)
            {
                Units.RemoveAt(i);
                result = true;
            }
        }

        return result;
    }

    /// <summary>
    /// 获取指定的单元类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetUnit<T>() where T: BaseFlowState
    {
        foreach(var a in Units)
        {
            if(a is T at)
            {
                return at;
            }
        }

        return null;
    }

    public T GetUnit<T>(string uid) where T : BaseFlowState
    {
        foreach (var a in Units)
        {
            if (a.GUID == uid)
            {
                return a as T;
            }
        }

        return null;
    }

    private void CheckCancellation()
    {
        GraphDestroyCancelToken.ThrowIfCancellationRequested();
    }

    #region 事件侦听，Cancel事件由GameHandlSystem调度处理
    async UniTask IFlowStateGraph.StartListener(IFlow flow)
    {
        foreach (var state in Units)
        {
            CheckCancellation();

            if (state.IsStart && state.IsListener == false)
            {
                if (state is IStateEventListener listener)
                {
                    listener.StartListener(flow);
                }

                await state.Enter(flow);
            }
        }
    }

    async UniTask IFlowStateGraph.StopListener(IFlow flow)
    {
        foreach (var state in Units)
        {
            CheckCancellation();

            if (state.IsListener == true)
            {
                if (state is IStateEventListener listener)
                {
                    listener.StopListener(flow);
                }

                await state.Exit(flow);
            }
        }
    }

    /// <summary>
    /// 生命周期下的Update
    /// </summary>
    internal async UniTask OnLoopUpdateState(IFlow flow)
    {
        foreach (var x in Units.Where(p => p.IsListener))
        {
            foreach(var e in x.EventListenerList.Where(p => p.IsListener))
            {
                if(e is IGameHanedleGraphEvent ghu)
                {
                    if (ghu.Execute(flow))
                    {
                        await ghu.CallFromGameHandleLoop(flow);
                    }
                }       
            }

            await x.Update(flow);

            CheckCancellation();
        }
    }

    internal async UniTask OnLoopUpdateTransition(Flow flow)
    {
        foreach (var x in Units.Where(p => p.IsListener))
        {
            foreach (var outTransition in x.AllOutTransition.Where(p => p.IsListener))
            {
                CheckCancellation();

                var result = await outTransition.Execute(flow);

                CheckCancellation();

                if (result == true)
                {
                    await outTransition.Branch(flow);
                }
            }
        }
    }
    #endregion
}
