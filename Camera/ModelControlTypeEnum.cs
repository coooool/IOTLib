using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IOTLib
{
    [System.Flags]
    public enum ModelControlTypeEnum
    {
        /// <summary>
        /// 无权限操作
        /// </summary>
        Null = 0,

        /// <summary>
        /// 可平移
        /// </summary>
        Translate = 1,

        /// <summary>
        /// 可旋转
        /// </summary>
        Rotate = 2,

        /// <summary>
        /// 绽放
        /// </summary>
        Scale = 4,

        /// <summary>
        /// 所有
        /// </summary>
        All = Translate | Rotate | Scale,
    }
}