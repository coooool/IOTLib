using System;
using System.Collections.Generic;
using System.Linq;

namespace IOTLib.Configure.ValueFactory
{
    public struct ValueMethodData
    {
        /// <summary>
        /// 变量名称
        /// </summary>
        public KeyValuePair<string, string>[] VarName { get; set; }

        /// <summary>
        /// 函数名
        /// </summary>
        public string Function { get; set; }

        /// <summary>
        /// 有调用Function?
        /// </summary>
        /// <returns></returns>
        public bool HasFunction()
        {
            return !string.IsNullOrEmpty(this.Function);
        }

        /// <summary>
        /// 获取所有变量，第一个参数是变量名， 第2个为默认值，如果没有填，则是String.Empty
        /// </summary>
        /// <param name="action"></param>
        public void GetVars(System.Action<string, string> action)
        {
            if (VarName == null || VarName.Length == 0) return;

            foreach (var a in VarName)
            {
                action?.Invoke(a.Key, a.Value);
            }
        }

        /// <summary>
        /// 转换值为字串数组
        /// </summary>
        /// <returns></returns>
        public string[] ValueParameters()
        {
            if (VarName == null || VarName.Length == 0) return Array.Empty<string>();

            return VarName.Select(a => a.Value).ToArray();
        }
    }
}