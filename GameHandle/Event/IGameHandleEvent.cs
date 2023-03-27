using System;
using UnityEngine;

namespace IOTLib
{
    public interface IGameHandleEvent : IEvent
    {
        /// <summary>
        /// 事件名称
        /// </summary>
        string EventName { get; }

        /// <summary>
        /// 侦听中？
        /// </summary>
        internal bool IsListener { get; set; }
    }
}
