using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Text;
using UnityEngine;
using static System.Net.Mime.MediaTypeNames;
using UnityEngine.Assertions;
using Newtonsoft.Json;

namespace IOTLib
{
    public static class ConvertUtility
    {
        private static JsonSerializerSettings JSONSETTING = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        /// <summary>
        /// 转换到原始分隔字符
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static string ToOriginStr(this Vector2 vec)
        {
            return vec.ToString().RemoveInvalidChars();
        }

        public static string ToOriginStr(this Vector3 vec)
        {
            return vec.ToString().RemoveInvalidChars();
        }

        public static string ToOriginStr(this Vector4 vec)
        {
            return vec.ToString().RemoveInvalidChars();
        }

        public static string RemoveInvalidChars(this string str)
        {
            Assert.IsFalse(string.IsNullOrEmpty(str), "无效的字符");

            return str.TrimStart('(').TrimEnd(')');
        }

        public static Vector2 ToVector2(this string str)
        {
            var pos_str = str.RemoveInvalidChars().Split(',');
           
            Assert.IsTrue(pos_str.Length == 2, $"无法还原的Vec2类型:{str}--->正确格式如下:0,0");

            return new Vector2(float.Parse(pos_str[0]), float.Parse(pos_str[1]));
        }

        public static Vector3 ToVector3(this string str)
        {
            var pos_str = str.RemoveInvalidChars().Split(',');

            Assert.IsTrue(pos_str.Length == 3, $"无法还原的Vec3类型:{str}--->正确格式如下:0,0,0");

            return new Vector3(float.Parse(pos_str[0]), float.Parse(pos_str[1]), float.Parse(pos_str[2]));
        }

        public static Vector4 ToVector4(this string str)
        {
            var pos_str = str.RemoveInvalidChars().Split(',');

            Assert.IsTrue(pos_str.Length == 4, $"无法还原的Vec4类型:{str}--->正确格式如下:0,0,0,0");

            return new Vector4(float.Parse(pos_str[0]), float.Parse(pos_str[1]), float.Parse(pos_str[2]), float.Parse(pos_str[3]));
        }

        /// <summary>
        /// 从一个JSON模板序填充目标对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json">JSON字符串</param>
        /// <returns>T类型</returns>
        public static T? ToObject<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json, JSONSETTING);
        }

        /// <summary>
        /// 从一个JSON模板填充一个匿名类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json">JSON字符串</param>
        /// <param name="type_template">填充模板</param>
        /// <returns></returns>
        public static T? ToAnonymousType<T>(this string json, T type_template)
        {
            return JsonConvert.DeserializeAnonymousType(json, type_template, JSONSETTING);
        }

        /// <summary>
        /// 序列化为JSON字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">目标</param>
        /// <returns></returns>
        public static string ToJson(this System.Object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.None, JSONSETTING);
        }

        /// <summary>
        /// 从十六进颜色代码转换到Color
        /// </summary>
        /// <param name="htmlColor">颜色信息</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">无法识别时的错误异常</exception>
        public static Color ToColor(this string htmlColor)
        {
            if(ColorUtility.TryParseHtmlString(htmlColor, out var c))
            {
                return c;
            }

            throw new InvalidOperationException($"无法转换的HTML颜色代码");
        }

        /// <summary>
        /// 动态识别颜色中的透明通道,如果是非透明只返回RGB，否则是RGBA
        /// </summary>
        /// <param name="color"></param>
        /// <param name="forceRGBA">强制转换为RGBA四通道十六进制代码</param>
        /// <returns></returns>
        public static string ToHtmlStringColor(this Color color, bool forceRGBA = false)
        {
            if(color.a == 0 && !forceRGBA)
            {
                return ColorUtility.ToHtmlStringRGB(color);
            }
            else
            {
                return ColorUtility.ToHtmlStringRGBA(color);
            }
        }
    }
}
