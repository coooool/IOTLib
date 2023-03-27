using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace IOTLib
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CGCustomEditorAttribute : Attribute
    {
        internal System.Type? CEType { get; private set; }

        public CGCustomEditorAttribute(System.Type type)
        {
            if (type.IsSubclassOf(typeof(CGCustomEditor)))
            {
                this.CEType = type;
            }
            else
            {
                Debug.LogError($"{type.Name}不是一个编辑器");
            }
        }
    }
}
