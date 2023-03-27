using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IOTLib;
using Cysharp.Threading.Tasks;

namespace IOTLib
{
    /// <summary>
    /// ����ͷ��״̬��ʲôҲ���ɡ��û�Ҳ�޷����������ת�Ȳ���
    /// </summary>
    public class EmptyState : FlowState
    {
        public const string TriggerEventName = "��״̬��ģʽ";

        public EmptyState() :base("��״̬")
        {

        }

        public override UniTask Enter(IFlow flow)
        {
            return base.Enter(flow);
        }

        public override UniTask Exit(IFlow flow)
        {
            return base.Exit(flow);
        }
    }
}