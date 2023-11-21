using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace IOTLib.GameHandle
{
    public struct FlowStateShapshot
    {
        private WeakReference<BaseFlowState> flowState;

        private uint m_id;

        public FlowStateShapshot(BaseFlowState state)
        {
            m_id = state.LastSnaphostId;
            flowState = new WeakReference<BaseFlowState>(state);
        }

        /// <summary>
        /// 状态是否发生过改变
        /// </summary>
        public readonly bool IsDirty
        {
            get
            {
                if(flowState.TryGetTarget(out var target))
                {
                    return target.LastSnaphostId != m_id;
                }

                return true;
            }
        }
    }
}
