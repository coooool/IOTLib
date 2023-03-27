using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.Events;
using IOTLib;

public class GameHandleEventSystem
{
    struct EventNode
    {
        public GameObject Target;
        public string EventName;
        public KeyValuePairs? FlowArgs;
        public UnityAction<IFlow> TriggerCallBack;
    }

    /// <summary>
    /// �����б�
    /// </summary>
    //private static Queue<EventNode> _eventList = new(8);

    private static object _lock = new object();

    /// <summary>
    /// ����һ����ҵ�¼�, ��LastUpdate֮ǰ���ö����ڱ�֡���з�֧������
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="graph_target">Ŀ�����</param>
    /// <param name="eventName">�¼���</param>
    /// <param name="flowArgs">����ʼ��һЩ����������ϵͳʹ��</param>
    /// <param name="triggerSuccessAction">������֧�ɹ����һ�λص�</param>
    public static void TriggerEvent(GameObject graph_target, string eventName, KeyValuePairs? flowArgs = null)
    {    
        lock (_lock)
        {
            if (graph_target == null)
            {
                Debug.LogWarning($"���� [{eventName}] �¼�ʧ�ܣ�Ŀ��ΪNULL");
                return;
            }

            MonoEventExecute.Execute<IGraphEventListen>(graph_target, null, (x, y) =>
            {
                if (x.TestEvent(eventName, out var igh))
                {
                    igh?.Trigger(flowArgs);
                }
            });

            //_eventList.Enqueue(new EventNode() { EventName = eventName, Target = target, FlowArgs = flowArgs, TriggerCallBack = triggerSuccessAction });
        }
    }

    public static void TriggerEvent(string targetGraphName, string eventName, KeyValuePairs? flowArgs = null)
    {
        var target = GameHandleSystem.GetFlowGraphFromName(targetGraphName);
        if(target == null)
        {
            Debug.LogWarning($"���� [{eventName}] �¼�ʧ�ܣ�Ŀ��ΪNULL");
            return;
        }

        TriggerEvent(target.gameObject, eventName, flowArgs);
    }

    /*
    public static void TriggerEvent(GameObject target, string eventName, UnityAction<IFlow> triggerSuccessAction = null)
    {
        TriggerEvent(target, eventName, null, triggerSuccessAction);
    }

    public static void TriggerEvent(GameObject target, string eventName, KeyValuePairs? flowArgs = null)
    {
        TriggerEvent(target, eventName, flowArgs, null);
    }

    internal static void SendEvent(IFlow flow)
    {
        lock (_lock)
        {
            while (_eventList.Count > 0)
            {
                var node = _eventList.Dequeue();

                ExecuteEvents.Execute<IGraphEventListen>(node.Target, null, (x, y) =>
                {
                    if (x.TestEvent(node.EventName, out var igh))
                    {
                        if (node.FlowArgs.HasValue)
                        {
                            while(node.FlowArgs.Value.Count > 0)
                            {
                                var arg = node.FlowArgs.Value.Pop();
                                flow.Vars.Set(arg.Key, arg.Value);
                            }
                        }

                        node.TriggerCallBack?.Invoke(flow);

                        igh.Trigger(flow);
                    }
                });
            }
        }
    }
    */
}
