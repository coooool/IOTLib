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
        /// 只有要存储过程才有值。
        /// </summary>
        internal Component Component { get; set; }

        /// <summary>
        /// 导出脚本到编辑器中
        /// </summary>
        /// <param name="fileId">文件ID</param>
        public CGEditor(string fileId, string customName = null)
        {
            FileId = fileId;
            CustomName = customName;
        }
    }
}