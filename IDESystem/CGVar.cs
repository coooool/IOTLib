using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace IOTLib
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class CGVar : Attribute
    {
        private string m_VarName;
        public string VarName
        {
            get
            {
                if (string.IsNullOrEmpty(m_VarName))
                {
                    return FieldInfo.Name;
                }

                return m_VarName;
            }
            set
            {
                m_VarName = value;
            }
        }

        public FieldInfo FieldInfo { get; set; }

        public CGVar()
        {
            this.VarName = null;
        }

        public CGVar(string name)
        {
            this.VarName = name;
        }
    }

    public struct VarGroup
    {
        public MonoBehaviour Behaviour { get; set; }
        public CGVar[] Vars { get; set; }

        internal CGCustomEditor? CustomEditor { get; set; }

        private string m_TypeName;

        public bool ToggleValue { get; set; }

        public string TypeName
        {
            get
            {
                if (string.IsNullOrEmpty(m_TypeName))
                {
                    if (Behaviour != null)
                        m_TypeName = Behaviour.GetType().Name;

                    if (string.IsNullOrEmpty(m_TypeName)) m_TypeName = "无法识别的类型";
                }

                return m_TypeName;
            }
        }
    }
}