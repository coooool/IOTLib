using System;
using System.Collections.Generic;
using System.Text;

namespace IOTLib
{
    public interface IEvent
    {
        /// <summary>
        /// 触发一个事件
        /// </summary>
        /// <param name="flow"></param>
        /// <returns></returns>
        void Trigger(KeyValuePairs? eventData);
    }
}
