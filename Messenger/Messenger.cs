// Messenger.cs v0.1 (20090925) by Rod Hyde (badlydrawnrod).
//
// This is a C# messenger (notification center) for Unity. It uses delegates
// and generics to provide type-checked messaging between event producers and
// event consumers, without the need for producers or consumers to be aware of
// each other.

using System;
using System.Collections.Generic;

namespace IOTLib
{
    /**
     * A messenger for events that have no parameters.
     */
    static public class Messenger
    {
        private static Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();

        static public Dictionary<string, Delegate> AddListener(string eventType, Callback handler)
        {
            // 线程安全锁
            lock (eventTable)
            {
                if (!eventTable.ContainsKey(eventType))
                {
                    eventTable.Add(eventType, handler);
                }
                else
                {
                    // 添加HANDLE到列表里
                    eventTable[eventType] = (Callback)eventTable[eventType] + handler;
                }
 
                return eventTable;
            }
        }

        static public void RemoveListener(string eventType, Callback handler)
        {
            // 线程安全锁
            lock (eventTable)
            {        
                if (eventTable.ContainsKey(eventType))
                {
                    Callback oldEvent = (Callback)eventTable[eventType];

                    if (oldEvent != null)
                    {
                        eventTable[eventType] = (oldEvent - handler)!;
                    }

                    if (eventTable[eventType] == null)
                    {
                        eventTable.Remove(eventType);
                    }
                }
            }
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        /// <param name="eventName">事件名</param>
        static public bool RemoveAllListenerFromName(string eventName)
        {
            lock (eventTable)
            {
                if (eventTable.ContainsKey(eventName))
                {
                    var ptr = eventTable[eventName];
                    
                    Delegate.RemoveAll(ptr, ptr);
                    
                    return eventTable.Remove(eventName);
                }
            }

            return false;
        }

        static public void Invoke(string eventType)
        {
            Delegate d;
          
            if (eventTable.TryGetValue(eventType, out d))
            {
                // 如果另一个线程要取消订阅此事件，则获取本地副本以防止出现竞争情况。 
                Callback callback = (Callback)d;
             
                if (callback != null)
                {
                    callback();
                }
            }
        }
    }


    /**
     * A messenger for events that have one parameter of type T.
     */
    static public class Messenger<T>
    {
        private static Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();

        static public Dictionary<string, Delegate> AddListener(string eventType, Callback<T> handler)
        {
            // 线程安全锁
            lock (eventTable)
            {
                if (!eventTable.ContainsKey(eventType))
                {
                    eventTable.Add(eventType, handler);
                }
                else
                {
                    eventTable[eventType] = (Callback<T>)eventTable[eventType] + handler;
                }
              
                return eventTable;
            }
        }

        static public void RemoveListener(string eventType, Callback<T> handler)
        {
            lock (eventTable)
            {
                if (eventTable.ContainsKey(eventType))
                {
                    Callback<T> oldEvent = (Callback<T>)eventTable[eventType];

                    if (oldEvent != null)
                    {
                        eventTable[eventType] = (oldEvent - handler)!;
                    }

                    if (eventTable[eventType] == null)
                    {
                        eventTable.Remove(eventType);
                    }
                }
            }
        }

        static public bool RemoveAllListenerFromName(string eventName)
        {
            lock (eventTable)
            {
                if (eventTable.ContainsKey(eventName))
                {
                    var ptr = eventTable[eventName];

                    Delegate.RemoveAll(ptr, ptr);

                    return eventTable.Remove(eventName);
                }
            }

            return false;
        }

        static public void Invoke(string eventType, T arg1)
        {
            Delegate d;

            if (eventTable.TryGetValue(eventType, out d))
            {
                Callback<T> callback = (Callback<T>)d;

                if (callback != null)
                {
                    callback(arg1);
                }
            }
        }
    }

    static public class Messenger<T, U>
    {
        private static Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();

        static public Dictionary<string, Delegate> AddListener(string eventType, Callback<T, U> handler)
        {
            // 线程安全锁
            lock (eventTable)
            {
                if (!eventTable.ContainsKey(eventType))
                {
                    eventTable.Add(eventType, handler);
                }
                else
                {
                    eventTable[eventType] = (Callback<T, U>)eventTable[eventType] + handler;
                }

                return eventTable;
            }
        }

        static public void RemoveListener(string eventType, Callback<T, U> handler)
        {
            lock (eventTable)
            {
                if (eventTable.ContainsKey(eventType))
                {
                    Callback<T,U> oldEvent = (Callback<T,U>)eventTable[eventType];

                    if (oldEvent != null)
                    {
                        eventTable[eventType] = (oldEvent - handler)!;
                    }

                    if (eventTable[eventType] == null)
                    {
                        eventTable.Remove(eventType);
                    }
                }
            }
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        /// <param name="eventName">事件名</param>
        static public bool RemoveAllListenerFromName(string eventName)
        {
            lock (eventTable)
            {
                if (eventTable.ContainsKey(eventName))
                {
                    var ptr = eventTable[eventName];

                    Delegate.RemoveAll(ptr, ptr);

                    return eventTable.Remove(eventName);
                }
            }

            return false;
        }

        static public void Invoke(string eventType, T arg1, U arg2)
        {
            Delegate d;
       
            if (eventTable.TryGetValue(eventType, out d))
            {
                Callback<T, U> callback = (Callback<T, U>)d;

                if (callback != null)
                {
                    callback(arg1, arg2);
                }
            }
        }
    }
}