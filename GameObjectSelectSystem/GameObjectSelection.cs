using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using UMOD;
using UnityEngine;
using UnityEngine.Events;

namespace IOTLib
{
    [GameSystem]
    [SystemDescribe(Author = "昊",
    Dependent = "无",
    Describe = "GameObject选择系统，在处理统一事件分发时使用",
    Name = "GameObjectSelection",
    Version = "0.1")]
    public class GameObjectSelection : BaseSystem
    {
        private static WeakReference<GameObject> m_ActiveGameObject;

        public static UnityEvent<GameObject> OnSelectChangedEvent;

        /// <summary>
        /// 获取当前激活的Object，如果没有返回null
        /// </summary>
        public static GameObject ActiveGameObject
        {
            get
            {
                if (m_ActiveGameObject == null) return null;

                if(m_ActiveGameObject.TryGetTarget(out var result))
                {
                    return result;
                }

                m_ActiveGameObject.SetTarget(null);
                return null;
            }

            set
            {
                if(m_ActiveGameObject == null) 
                    m_ActiveGameObject= new WeakReference<GameObject>(value);
                else
                m_ActiveGameObject.SetTarget(value);

                if(value != null)
                {
                    OnSelectChangedEvent?.Invoke(value);
                    SystemManager.GetSystem<GameObjectSelection>().TriggerBindEvent(value);
                }
            }
        }

        public override void BindData(DataBehaviour monoBehaviour)
        {
            if (monoBehaviour is IGameObjectSelectEvent)
            {
                base.BindData(monoBehaviour);
            }
            else
            {
                Debug.LogError("GameObjectSelect不接受未实现IGameObjectSelectEvent的数据绑定");
            }
        }

        internal void TriggerBindEvent(GameObject target)
        {
            foreach(var a in this)
            {
                if (a is IGameObjectSelectEvent gse)
                    gse.OnSelectGameObject(target);
            }
        }
    }
}
