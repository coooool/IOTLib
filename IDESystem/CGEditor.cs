using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IOTLib
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CGEditor : Attribute
    {
        internal string FileId;

        internal string CustomName;
        /// <summary>
        /// ֻ��Ҫ�洢���̲���ֵ��
        /// </summary>
        internal Component Component { get; set; }

        /// <summary>
        /// �����ű����༭����
        /// </summary>
        /// <param name="fileId">�ļ�ID</param>
        public CGEditor(string fileId, string customName = null)
        {
            FileId = fileId;
            CustomName = customName;
        }
    }
}