using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace IOTLib
{
    public class MonoEventExecute
    {
        public delegate void EventFunction<T1>(T1 handler, BaseMonoEventData eventData);

        private static void GetEventChain(GameObject root, IList<Transform> eventChain)
        {
            eventChain.Clear();
            if (root == null)
                return;

            var t = root.transform;
            while (t != null)
            {
                eventChain.Add(t);
                t = t.parent;
            }
        }

        public static bool Execute<T>(GameObject target, BaseMonoEventData eventData, EventFunction<T> functor) where T : IMonoEventHandler
        {
            var internalHandlers = ListPool<IMonoEventHandler>.Get();
            GetEventList<T>(target, internalHandlers);

            var internalHandlersCount = internalHandlers.Count;
            for (var i = 0; i < internalHandlersCount; i++)
            {
                T arg;
                try
                {
                    arg = (T)internalHandlers[i];
                }
                catch (Exception e)
                {
                    var temp = internalHandlers[i];
                    Debug.LogException(new Exception(string.Format("类型 {0} 找不到 {1} 接收器.", typeof(T).Name, temp.GetType().Name), e));
                    continue;
                }

                try
                {
                    functor(arg, eventData);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }

            var handlerCount = internalHandlers.Count;
            ListPool<IMonoEventHandler>.Release(internalHandlers);
            return handlerCount > 0;
        }

        private static readonly List<Transform> s_InternalTransformList = new List<Transform>(30);

        public static GameObject? ExecuteHierarchy<T>(GameObject root, BaseMonoEventData eventData, EventFunction<T> callbackFunction) where T : IMonoEventHandler
        {
            GetEventChain(root, s_InternalTransformList);

            var internalTransformListCount = s_InternalTransformList.Count;
            for (var i = 0; i < internalTransformListCount; i++)
            {
                var transform = s_InternalTransformList[i];
                if (Execute(transform.gameObject, eventData, callbackFunction))
                    return transform.gameObject;
            }

            return null;
        }

        private static bool ShouldSendToComponent<T>(Component component) where T : IMonoEventHandler
        {
            var valid = component is T;
            if (!valid)
                return false;

            var behaviour = component as Behaviour;
            if (behaviour != null)
                return behaviour.isActiveAndEnabled;
            return true;
        }

        private static void GetEventList<T>(GameObject go, IList<IMonoEventHandler> results) where T : IMonoEventHandler
        {
            if (results == null)
                throw new ArgumentException("获取的结果为空", "返回");

            if (go == null || !go.activeInHierarchy)
                return;

            var components = ListPool<Component>.Get();
            go.GetComponents(components);

            var componentsCount = components.Count;
            for (var i = 0; i < componentsCount; i++)
            {
                if (!ShouldSendToComponent<T>(components[i]))
                    continue;

                results.Add((IMonoEventHandler)components[i]);
            }

            ListPool<Component>.Release(components);
        }

        public static bool CanHandleEvent<T>(GameObject go) where T : IMonoEventHandler
        {
            var internalHandlers = ListPool<IMonoEventHandler>.Get();
            GetEventList<T>(go, internalHandlers);
            var handlerCount = internalHandlers.Count;
            ListPool<IMonoEventHandler>.Release(internalHandlers);
            return handlerCount != 0;
        }

        public static GameObject? GetEventHandler<T>(GameObject root) where T : IMonoEventHandler
        {
            if (root == null)
                return null;

            Transform t = root.transform;
            while (t != null)
            {
                if (CanHandleEvent<T>(t.gameObject))
                    return t.gameObject;
                t = t.parent;
            }

            return null;
        }
    }
}
