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
    /// ǿ���˳�AnyState�����
    /// </summary>
    public bool AllowExit;

    private bool m_IsMirrorState;
    /// <summary>
    /// �Ǿ���ڵ�
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
    /// ��ȡ������������
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
        // AnyState��û��ǿ�Ʋ����²����г����� 
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
            Debug.LogError("�ڴ淢���˴�����ͷ�...����һ����ʹ�Դ���");
            return false;
        }

        result = findState;

        return findState.IsMirrorState == false;
    }

    /// <summary>
    /// �����������״̬���ڽ��з�֧�л��������Ӵ���
    /// </summary>
    /// <param name="targetState"></param>
    public bool AddMirrorState(AnyState targetState)
    {
        if (TryGetRootState(this, out var root))
        {
            if(root == targetState)
            {
                Debug.LogError("�޷���������");
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
    /// �������жϿ�һ����������״̬
    /// </summary>
    /// <param name="anyState"></param>
    public bool RemoveMirrorState(AnyState targetState)
    {
        if(TryGetRootState(targetState, out var root))
        {
            if (root == targetState)
            {
                Debug.LogError("�޷�ɾ����������");
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
