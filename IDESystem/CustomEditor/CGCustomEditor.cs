using System;
using System.Collections.Generic;
using UnityEngine;

namespace IOTLib
{
    public abstract class CGCustomEditor : ICGCustomEditor, IComponentPropertyUpdate
    {
        public Component target { get; internal set; }

        public abstract void OnEnable();

        public virtual void OnGUI(Behaviour instance, IEnumerable<CGVar> vars)
        {
            CGPrefabPropertyDrawUtility.DrawFields(target, vars);
        }

        public virtual void OnDragPerform(ExportCGPrefab prefab)
        {

        }

        public virtual void OnUpdate()
        {
        }

        public virtual void OnDestroy() { }

        /// <summary>
        /// 值发生更新
        /// </summary>
        public virtual void OnCGFieldUpdate()
        {
        
        }
    }
}
