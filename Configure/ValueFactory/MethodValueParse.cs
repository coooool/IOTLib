using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Assertions;

namespace IOTLib.Configure.ValueFactory
{
    public static class MethodValueParse
    {
        private static Regex MNV = new Regex(@"{((\w+):)?(\S+)?}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// 解码一个方法值块
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool Parse(string text, out ValueMethodData data)
        {
            data = default(ValueMethodData);

            Assert.IsFalse(string.IsNullOrEmpty(text), "输入了空的值分析数据");

            var mats = MNV.Match(text);

            if (mats == null)
            {
                Debug.LogWarning($"{text}绑定语法错误");
                return false;
            }
            else
            {
                // 2是方法名,3是变量名
                bool hasFunction = false;
                // 添加了动态方法?
                if (mats.Groups[2].Success)
                {
                    hasFunction = true;
                    data.Function = mats.Groups[2].Value;
                }

                var parameter_and_value = mats.Groups[3].Value.Split(',');

                if (!hasFunction) Assert.IsTrue(parameter_and_value.Length == 1, "语法错误,非函数语法只能出现一个变量名");
                else Assert.IsTrue(parameter_and_value.Length == 0, "语法错误,函数绑定参数至少1个");

                data.VarName = new KeyValuePair<string, string>[parameter_and_value.Length];

                if (parameter_and_value != null && parameter_and_value.Length > 0)
                {
                    for (var i = 0; i < parameter_and_value.Length; i++)
                    {
                        var name_val = parameter_and_value[i].Split('?');

                        if (name_val.Length == 1)
                            data.VarName[i] = new KeyValuePair<string, string>(name_val[0], string.Empty);
                        else if (name_val.Length == 2)
                            data.VarName[i] = new KeyValuePair<string, string>(name_val[0], name_val[1]);
                        else throw new InvalidOperationException("错误的参数绑定形式,正确的应如a?b");
                    }
                }

                return true;
            }
        }
    }
}