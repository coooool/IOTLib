using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace IOTLib
{
    public static class TransitionUtility 
    {
        /// <summary>
        /// ����һ������ί�д����ı任��
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="ExecuteAction"></param>
        /// <returns></returns>
        public static FlowStateTransitionAction CreateAction(IFlowState source, IFlowState target, System.Threading.CancellationToken GraphLiftCancellation, System.Func<IFlow, System.Threading.CancellationToken, UniTask<bool>> ExecuteAction)
        {
            return new FlowStateTransitionAction(source, target, GraphLiftCancellation, ExecuteAction);
        }

        /// <summary>
        /// ����һ���¼������Ա任��
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="triggerName"></param>
        /// <returns></returns>
        public static FlowStateTransitionEventTrigger CreateEvent(IFlowState source, IFlowState target, string triggerName)
        {
            return new FlowStateTransitionEventTrigger(source, target, triggerName);
        }

        /// <summary>
        /// ��ʱһ�κ��Զ��任��֧,�ñ��巽�����ں�̨��ʱ��������ϵͳ��
        /// </summary>
        /// <param name="source">��Դ�ڵ�</param>
        /// <param name="target">Ŀ��ڵ�</param>
        /// <param name="delay">�������ʱ��</param>
        public static DelayTimeTransition CreateDelayTransition(IFlowState source, IFlowState target, float seconds)
        {
            return new DelayTimeTransition(source, target, seconds);
        }

        /// <summary>
        /// ��ʱһ�κ��Զ��任��֧,�ñ��巽�������첽��ʱ��������ϵͳ��
        /// </summary>
        /// <param name="source">��Դ�ڵ�</param>
        /// <param name="target">Ŀ��ڵ�</param>
        /// <param name="delay">�������ʱ��</param>
        /// <param name="graphLiftToken">��������</param>
        public static DelayTimeTransition CreateDelayTransition(IFlowState source, IFlowState target, float seconds, System.Threading.CancellationToken GraphLiftCancellation)
        {
            return new DelayTimeTransition(source, target, seconds, GraphLiftCancellation);
        }
    }
}
