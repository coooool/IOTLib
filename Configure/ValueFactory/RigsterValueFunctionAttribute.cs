using System;
using System.Collections.Generic;
using System.Text;

namespace IOTLib.Configure.ValueFactory
{
    /// <summary>
    /// 注册一个值提供工厂的Function转换器
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RigsterValueFunctionAttribute : Attribute
    {
        public string FunctionName { get; set; }

        /// <summary>
        /// 注册方法
        /// </summary>
        /// <param name="funcName">函数名</param>
        public RigsterValueFunctionAttribute(string funcName)
        {
            FunctionName = funcName;
        }
    }
}
