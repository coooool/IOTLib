using IOTLib.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UMOD;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace IOTLib.Configure.ValueFactory
{
    public class ValueProviderFactory
    {
        public delegate string ValueProviderFuncDelegate(string key);

        private static List<ValueProviderNode> ValueProvider = new();
        private static ValueProviderFuncDelegate? ValueProviderFunc;

        /// <summary>
        /// 注册一个提供器
        /// </summary>
        /// <param name="factoryName"></param>
        /// <param name="provider"></param>
        public static void RegisterValueProvider(string factoryName, IConfigureValueProvider provider)
        {
            var exist = ValueProvider.FindIndex(p => p.Name == factoryName);
            if (exist == -1)
            {
                ValueProvider.Add(new ValueProviderNode { Name = factoryName, ValueProvider = provider });
            }
        }

        /// <summary>
        /// 注册一个提供器
        /// </summary>
        /// <param name="provider"></param>
        public static void RegisterValueProvider(ValueProviderFuncDelegate provider)
        {
            ValueProviderFunc += provider;
        }

        /// <summary>
        /// 卸载一个提供器
        /// </summary>
        /// <param name="factoryName"></param>
        public static void UnRegisterValueProvider(string factoryName)
        {
            var exist = ValueProvider.FindIndex(p => p.Name == factoryName);
            if (exist != -1)
            {
                ValueProvider.RemoveAt(exist);
            }
        }

        /// <summary>
        /// 卸载一个提供器
        /// </summary>
        /// <param name="provider"></param>
        public static void UnRegisterValueProvider(ValueProviderFuncDelegate provider)
        {
            ValueProviderFunc -= provider;
        }

        private static bool GetBaseSystemRegisterValueFunctionAttribute(string funcName, out object outValue, SystemGroup systemGroup, params object[] parameater)
        {
            outValue = null;

            foreach (var a in systemGroup)
            {
                if (ReflectionUtility.InvokeMethodFromAttribute<RigsterValueFunctionAttribute>(
                    BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    a,
                    (attr) => attr.FunctionName == funcName,
                    out outValue,
                    parameater
                    ))
                        return true;
            }

            return false;
        }

        public static bool InvokeDynFunction(string methodName, out object outValue, params object[] value)
        {
            if (GetBaseSystemRegisterValueFunctionAttribute(methodName, out outValue, Single<InitializationSystemGroup>.Instance, value))
                return true;

            if (GetBaseSystemRegisterValueFunctionAttribute(methodName, out outValue, Single<SimulationSystemGroup>.Instance, value))
                return true;

            if (GetBaseSystemRegisterValueFunctionAttribute(methodName, out outValue, Single<LastUpdateSystemGroup>.Instance, value))
                return true;

            return false;
        }

        /// <summary>
        /// 获取一个属性值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool GetValueFromProvider(string key, out string value)
        {
            return GetValueFromProvider(key, null, out value);
        }

        public static bool GetValueFromProvider(string key, KeyValuePairs? customParamter, out string value)
        {
            value = string.Empty;

            ValueMethodData methodData;
            if(!MethodValueParse.Parse(key, out methodData))
            {
                Debug.Log($"解析语法失败(错误的格式):{key}");
                return false;
            }

            var pb = new ParameterBindParse(methodData.VarName.Length);

            // -----------------------------------------
            // 从自定义参数中查找 
            // -----------------------------------------
            if (customParamter.HasValue)
            {
                pb.BindValuFromKeyPairs(customParamter.Value, methodData.VarName);
            }

            // -----------------------------------------
            // 从注册器中查找 
            // -----------------------------------------
            if (pb.BindSuccessCount < methodData.VarName.Length && ValueProviderFunc != null)
            {
                var funcList = ValueProviderFunc.GetInvocationList();
                foreach (ValueProviderFuncDelegate method in funcList)
                {
                    pb.BindValueFromFuncDelegate(method, methodData.VarName);
                }
            }
       
            if (pb.BindSuccessCount < methodData.VarName.Length)
            {
                foreach (var a in ValueProvider)
                {
                    pb.BindValueFromValueProviderNode(a.ValueProvider, methodData.VarName);
                }
            }
            // -----------------------------------------

            // -----------------------------------------
            // 尝试恢复默认参数
            // -----------------------------------------
            if (pb.BindSuccessCount < methodData.VarName.Length)
            {
                pb.BindValueFromDefaultValue(methodData.VarName);
            }

            // -----------------------------------------
            // 已经成功绑定了足够的参数? 
            // -----------------------------------------
            if (pb.BindSuccessCount == methodData.VarName.Length)
            {
                if (methodData.HasFunction())
                {
                    // 从网络编辑器模式下进来不调用函数
                    if (customParamter.HasValue)
                        if (customParamter.Value.TryGetValue("IsEditorMode---IOTLIBEDITOR", out var _))
                        {
                            value = string.Empty;
                            //value = pb.GetDefaultValue(key);
                            return true;
                        }

                    if (InvokeDynFunction(methodData.Function, out var result, pb.ValueParameters()))
                    {
                        value = result.ToString();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    value = pb.GetValueAtIndex(0);
                    return true;
                }
            }
     
            return false;
        }
    }
}
