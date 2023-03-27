using System;
using System.Collections.Generic;
using UnityEngine;

namespace IOTLib
{
    public interface IGameObjectSelectEvent
    {
        /// <summary>
        /// 在重新设置激活GameObject时会触发这个事件
        /// </summary>
        /// <param name="gameObject"></param>
        void OnSelectGameObject(GameObject gameObject);
    }
}
