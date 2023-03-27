using System;
using UnityEngine;

namespace IOTLib
{
    public abstract class IMonoAbstractEventData
    {
        protected bool m_Used;

        /// <summary>
        /// 复位
        /// </summary>
        public virtual void Reset()
        {
            m_Used = false;
        }

        /// <summary>
        /// 使用这个值 
        /// </summary>
        /// <remarks>
        /// 在事件冒泡中如果这个事件已经被使用则不再继续广播
        /// </remarks>
        public virtual void Use()
        {
            m_Used = true;
        }

        /// <summary>
        /// 这个事件已经被事件了？
        /// </summary>
        public virtual bool used
        {
            get { return m_Used; }
        }
    }
}
