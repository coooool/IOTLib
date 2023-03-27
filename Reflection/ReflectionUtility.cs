using IOTLib.Configure.ValueFactory;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace IOTLib.Reflection
{
    public static class ReflectionUtility
    {
        /// <summary>
        /// 从指定类型上获取指定属性，存在True，反之False
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inherit"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryGetTypeAttritube<T>(System.Type type, bool inherit, out T result) where T: Attribute
        {
            result = type.GetCustomAttribute<T>(inherit);
            if(result == null) return false;

            return true;
        }

        /// <summary>
        /// 查找一个类型下的实现了指定Attribute的方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bindingFlags">方法过虑</param>
        /// <param name="type">实例</param>
        /// <param name="checkCallback">回调属性，返回True则调用。FALSE则不调用</param>
        /// <param name="returnValue">返回调用的返回的值</param>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        public static bool InvokeMethodFromAttribute<T>(
            BindingFlags bindingFlags,
            object type,
            System.Func<T?, bool> checkCallback,
            out object returnValue,
            params object[] parameter
            ) where T : Attribute
        {
            returnValue = null;
            string methodName = "未命名";

            try
            {
                var methods = type.GetType().GetMethods(bindingFlags);

                if (methods != null && methods.Length > 0)
                {
                    foreach (var m in methods)
                    {
                        var method_fields = m.GetCustomAttribute<T>(false);

                        if (method_fields != null)
                        {
                            if (checkCallback.Invoke(method_fields))
                            {
                                methodName = m.Name;
                                returnValue = m.Invoke(type, parameter);
                                return true;
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Debug.LogError($"调用目标函数{methodName}失败，请检查参数是否匹配");
                Debug.LogError(e.Message);
            }

            return false;
        }
    }
}
