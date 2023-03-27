using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;

namespace IOTLib
{
    /// <summary>
    /// 基于Lambda创建创建一个快速的作业
    /// </summary>
    public class FlowStateAction : BaseFlowState
    {
        private Func<CancellationToken, UniTask> m_onEnter;
        private Func<CancellationToken, UniTask> m_onExit;

        /// <summary>
        /// 基于lambda快速创建一个状态
        /// </summary>
        /// <param name="stateName">状态名</param>
        /// <param name="_in_enter">进入状态回调</param>
        /// <param name="_in_exit">离开状态回调</param>
        /// <returns>状态实例</returns>
        public static FlowStateAction Create(string stateName, Func<CancellationToken, UniTask> _in_enter, Func<CancellationToken, UniTask> _in_exit)
        {
            return new FlowStateAction(stateName, _in_enter, _in_exit);
        }

        /// <summary>
        /// 基于lambda快速创建一个状态
        /// </summary>
        /// <param name="stateName">状态名</param>
        /// <param name="_in_enter">进入状态回调</param>
        /// <param name="_in_exit">离开状态回调</param>
        public FlowStateAction(string stateName, Func<CancellationToken, UniTask> _in_enter, Func<CancellationToken, UniTask> _in_exit) : base(stateName)
        {
            m_onEnter= _in_enter;
            m_onExit= _in_exit;
        }

        public override async UniTask Enter(IFlow flow)
        {
            var task = m_onEnter?.Invoke(DestroyOrExitStateCancelToken);

            if(task.HasValue)
                await task.Value;
        }

        public override async UniTask Exit(IFlow flow)
        {
            var task = m_onExit?.Invoke(DestroyOrExitStateCancelToken);

            if (task.HasValue)
                await task.Value;
        }
    }
}
