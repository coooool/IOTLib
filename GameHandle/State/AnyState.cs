using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class AnyState : BaseFlowState
{
    override public bool IsStart => true;

    /// <summary>
    /// 强制退出AnyState命令符
    /// </summary>
    public bool AllowExit;

    private bool m_IsMirrorState;
    /// <summary>
    /// 是镜像节点
    /// </summary>
    protected bool IsMirrorState => m_IsMirrorState;

    private WeakReference<AnyState> m_TargetMirrorInstance = null;

    private List<WeakReference<AnyState>> m_ChildMirrorState = null;
    protected List<WeakReference<AnyState>> ChildMirrorState
    {
        get
        {
            if(m_ChildMirrorState == null)
                m_ChildMirrorState = new List<WeakReference<AnyState>>();

            return m_ChildMirrorState;
        }
    }

    public AnyState(string stateName = null) : base(stateName)
    {
        m_IsMirrorState = false;
    }

    /// <summary>
    /// 获取所有虚拟链接
    /// </summary>
    /// <param name="callBack"></param>
    protected void GetAllChildState(System.Action<AnyState> callBack)
    {
        if (m_ChildMirrorState != null)
        {
            for (var i = m_ChildMirrorState.Count - 1; i >= 0; i--)
            {
                var link = m_ChildMirrorState[i];
                if (link.TryGetTarget(out var node))
                {
                    callBack?.Invoke(node);
                }
                else
                {
                    m_ChildMirrorState.RemoveAt(i);
                }
            }
        }
    }

    public override UniTask Enter(IFlow flow)
    {
        if (!IsMirrorState)
        {
            GetAllChildState(p =>
            {
                p.EnterAndListener(flow).Forget();
            });
        }

        return UniTask.CompletedTask;
    }

    public override async UniTask Exit(IFlow flow)
    {
        if (!IsMirrorState)
        {
            GetAllChildState(p =>
            {
                p.ExitAndStopListener(flow).Forget();
            });
        }

        var @this = this as IFlowState;

        //var parentTransition = flow.TransitionStack.Peek();
        //flow.Vars.Set("IsFromAnyState", true);

        foreach (var transition in @this.AllOutTransition)
        {
            await IteraExitIFlowState(transition.DestinationState, flow);
        }

        //if (lastActiveState != null && lastActiveState.IsAlive == true)
        //    lastActiveState.Target = parentTransition.DestinationState;
        //else
        //    lastActiveState = new WeakReference(parentTransition.DestinationState);
    }

    private async UniTask IteraExitIFlowState(IFlowState state, IFlow flow)
    {
        await state.ExitAndStopListener(flow); 

        foreach (var x in state.AllOutTransition)
        {
            await IteraExitIFlowState(x.DestinationState, flow);
        }
    }

    protected override void StopListener(IFlow flow)
    {
        // AnyState在没有强制参数下不进行出操作 
        if (AllowExit)
        {
            base.StopListener(flow);
        }
    }

    internal bool TryGetRootState(AnyState anyState, out AnyState result)
    {
        var findState = anyState;
        result = null;

        while (findState.IsMirrorState)
        {
            if (findState.m_TargetMirrorInstance.TryGetTarget(out var parent))
            {
                findState = parent;
            }
            else
            {
                findState = null;
                break;
            }
        }

        if (findState == null)
        {
            Debug.LogError("内存发生了错误的释放...这是一个致使性错误");
            return false;
        }

        result = findState;

        return findState.IsMirrorState == false;
    }

    /// <summary>
    /// 添加虚拟链接状态，在进行分支切换会行链接处理
    /// </summary>
    /// <param name="targetState"></param>
    public bool AddMirrorState(AnyState targetState)
    {
        if (TryGetRootState(this, out var root))
        {
            if(root == targetState)
            {
                Debug.LogError("无法镜像自身");
                return false;
            }

            targetState.m_IsMirrorState = true;
            targetState.m_TargetMirrorInstance = new WeakReference<AnyState>(root);
            root.ChildMirrorState.Add(new WeakReference<AnyState>(targetState));

            return true;
        }

        return false;
    }

    /// <summary>
    /// 从主体中断开一个虚拟链接状态
    /// </summary>
    /// <param name="anyState"></param>
    public bool RemoveMirrorState(AnyState targetState)
    {
        if(TryGetRootState(targetState, out var root))
        {
            if (root == targetState)
            {
                Debug.LogError("无法删除镜像自身");
                return false;
            }

            var idx = root.m_ChildMirrorState.FindIndex(p =>
            {
                if (p.TryGetTarget(out var r))
                {
                    return r == targetState;
                }

                return false;
            });

            if (idx >= 0)
            {
                root.m_ChildMirrorState.RemoveAt(idx);
                return true;
            }
        }

        return false;
    }
}
