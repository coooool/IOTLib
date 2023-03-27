using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace IOTLib
{
    public static class TransitionUtility 
    {
        /// <summary>
        /// 创建一个基本委托创建的变换器
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
        /// 创建一个事件触发性变换器
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
        /// 定时一段后自动变换分支,该变体方法基于后台计时，不阻塞系统。
        /// </summary>
        /// <param name="source">来源节点</param>
        /// <param name="target">目标节点</param>
        /// <param name="delay">基于秒的时间</param>
        public static DelayTimeTransition CreateDelayTransition(IFlowState source, IFlowState target, float seconds)
        {
            return new DelayTimeTransition(source, target, seconds);
        }

        /// <summary>
        /// 定时一段后自动变换分支,该变体方法基于异步计时，会阻塞系统。
        /// </summary>
        /// <param name="source">来源节点</param>
        /// <param name="target">目标节点</param>
        /// <param name="delay">基于秒的时间</param>
        /// <param name="graphLiftToken">生命周期</param>
        public static DelayTimeTransition CreateDelayTransition(IFlowState source, IFlowState target, float seconds, System.Threading.CancellationToken GraphLiftCancellation)
        {
            return new DelayTimeTransition(source, target, seconds, GraphLiftCancellation);
        }
    }
}
