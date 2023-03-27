using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace IOTLib
{
    public class MessengerAgent : MonoBehaviour
    {
        private struct RegEventNode
        {
            public string name;
            public Dictionary<string, Delegate> func;
        }

        private List<RegEventNode>? m_eventNames;

        private List<RegEventNode> Names
        {
            get
            {
                if (m_eventNames == null) m_eventNames = new List<RegEventNode>();
                return m_eventNames;
            }
        }

        public bool AddEventName(string name, Dictionary<string, Delegate> refObj)
        {
            var target = Names.Find(p => p.name == name);

            if (string.IsNullOrEmpty(target.name))
            { 
                Names.Add( new RegEventNode() { name = name, func = refObj });
                return true;
            }

            return false;
        }

        public void RemoveEventName(string name) {
            for(var i = Names.Count - 1; i >= 0; i--)
            {
                var node = Names[i];

                if (node.func.TryGetValue(name, out var func))
                {
                    foreach (var ppp in func.GetInvocationList())
                    {
                        Delegate.Remove(func, ppp);
                    }

                    node.func.Remove(node.name);
                    Names.RemoveAt(i);
                }
            }
        }

        private void OnDestroy()
        {
            if(m_eventNames != null)
            {
                foreach(var node in m_eventNames)
                {
                    if( node.func.TryGetValue(node.name, out var func) )
                    {
                        foreach(var ppp in func.GetInvocationList())
                        {
                            Delegate.Remove(func, ppp);
                        }          
                    }
                    node.func.Remove(node.name);
                }
            }
        }
    }

}