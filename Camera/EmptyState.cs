using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IOTLib;
using Cysharp.Threading.Tasks;

namespace IOTLib
{
    /// <summary>
    /// 摄像头空状态，什么也不干。用户也无法操作鼠标旋转等操作
    /// </summary>
    public class EmptyState : FlowState
    {
        public const string TriggerEventName = "无状态空模式";

        public EmptyState() :base("空状态")
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