using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IOTLib
{
    [Serializable]
    public struct Vector2D
    {
        [SerializeField]
        public double x;
        [SerializeField]
        public double y;

        public Vector2D(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return string.Format("{0},{1}", y, x);
        }

        /// <summary>
        /// 转换一个字符串为DOUBLE型VEC2
        /// </summary>
        /// <param name="str">输入格式为:经度,纬度</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static Vector2D Parse(string str)
        {
            var posStr = str.Split(',');
            if (posStr.Length != 2) throw new InvalidOperationException("无效的输入参数");
            return new Vector2D(double.Parse(posStr[1]), double.Parse(posStr[0]));
        }
    }
}