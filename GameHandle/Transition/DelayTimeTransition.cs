using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace IOTLib
{
    /// <summary>
    /// 延迟自动变换节点
    /// </summary>
    public class DelayTimeTransition : FlowStateTransition
    {
        private System.Threading.CancellationToken? cancelToken;

        private float m_cacheTime = 0;
        private float m_delayTime = 1;

        /// <summary>
        /// 定时一段后自动变换分支,该变体方法基于后台计时，不阻塞系统。
        /// </summary>
        /// <param name="source">来源节点</param>
        /// <param name="target">目标节点</param>
        /// <param name="delay">基于秒的时间</param>
        public DelayTimeTransition(IFlowState source, IFlowState target, float delay) : base(source, target)
        {
            cancelToken = null;
            m_delayTime= delay;
        }

        /// <summary>
        /// 定时一段后自动变换分支,该变体方法基于异步计时，会阻塞系统。
        /// </summary>
        /// <param name="source">来源节点</param>
        /// <param name="target">目标节点</param>
        /// <param name="delay">基于秒的时间</param>
        /// <param name="graphLiftToken">状态的生命周期</param>
        public DelayTimeTransition(IFlowState source, IFlowState target, float delay, System.Threading.CancellationToken graphLiftToken) : base(source, target)
        {
            cancelToken = graphLiftToken;
            m_delayTime = delay;
        }

        protected override void OnInitialization(IFlow flow)
        {
            m_cacheTime = 0;
            base.OnInitialization(flow);
        }

        protected override void OnUnitialization(IFlow flow)
        {
            cancelToken= null;
            base.OnUnitialization(flow);
        }

        public override async UniTask<bool> Execute(Flow flow)
        {
            if (cancelToken.HasValue)
            {
                var delTIme = Mathf.RoundToInt(m_delayTime * 1000.0f);
                await UniTask.Delay(delTIme, false, PlayerLoopTiming.Update, cancelToken.Value);

                return true;
            }
            else
            {
                m_cacheTime += Time.deltaTime;

                if (m_cacheTime > m_delayTime)
                {
                    m_cacheTime = 0;

                    return true;
                }
            }

            return false;
        }
    }
}
