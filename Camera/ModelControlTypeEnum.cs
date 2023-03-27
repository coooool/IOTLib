using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IOTLib
{
    [System.Flags]
    public enum ModelControlTypeEnum
    {
        /// <summary>
        /// ��Ȩ�޲���
        /// </summary>
        Null = 0,

        /// <summary>
        /// ��ƽ��
        /// </summary>
        Translate = 1,

        /// <summary>
        /// ����ת
        /// </summary>
        Rotate = 2,

        /// <summary>
        /// ����
        /// </summary>
        Scale = 4,

        /// <summary>
        /// ����
        /// </summary>
        All = Translate | Rotate | Scale,
    }
}