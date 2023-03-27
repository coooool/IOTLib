using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IOTLib
{
    /// <summary>
    /// 标记导出CG资源
    /// </summary>
    [IOTLib.MonoHeaderHelp("标记导出资源，加入到运行时模型库中")]
    public class ExportCGPrefab : MonoBehaviour
    {
        [Header("资源图标")]
        public Texture2D m_CgIco;
        [Header("资源名称")]
        public string m_CgName;
        [Header("资源描述")]
        public string m_CgDescription;
        [Header("资产类型")]
        public string m_CgType;

        [SerializeField, HideInInspector]
        public int SourceInstanceID { get; set; }

        [SerializeField, HideInInspector]
        internal string m_PrefabID;

        public Texture GetIco()
        {
            if (m_CgIco == null)
            {
                return Resources.Load<Texture>("DOS/cg_prefab_noname");
            }
            else
            {
                return m_CgIco;
            }
        }

        public string GetCgName()
        {
            return string.IsNullOrEmpty(m_CgName) ? "(未命名的东西)" : m_CgName;
        }

        public string GetCgDescription()
        {
            return string.IsNullOrEmpty(m_CgDescription) ? "(无)" : m_CgDescription;
        }

        /// <summary>
        /// 如果为空，返回未分类
        /// </summary>
        /// <returns></returns>
        public string GetSafeCgType()
        {
            return string.IsNullOrEmpty(m_CgType) ? "未分类" : m_CgType;
        }
    }
}