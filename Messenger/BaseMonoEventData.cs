using System;
using System.Collections.Generic;

namespace IOTLib
{
    /// <summary>
    /// 基于的Mono事件数据
    /// </summary>
    public class BaseMonoEventData : IMonoAbstractEventData
    {
        //public static readonly BaseMonoEventData Empty = new BaseMonoEventData();

        /// <summary>
        /// 事件ID
        /// </summary>
        public virtual uint EventID { get; } = 0;

        /// <summary>
        /// 事件数据
        /// </summary>
        public System.Object? Data { get; } = null;

        /// <summary>
        /// 创建一个事件数据传递类
        /// </summary>
        /// <param name="eventID">事件ID</param>
        /// <param name="data">事件数据</param>
        public BaseMonoEventData(uint eventID, System.Object? data)
        {
            EventID = eventID; 
            Data = data;
        }
    }
}
