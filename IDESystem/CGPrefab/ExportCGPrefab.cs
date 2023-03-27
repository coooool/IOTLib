using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IOTLib
{
    /// <summary>
    /// ��ǵ���CG��Դ
    /// </summary>
    [IOTLib.MonoHeaderHelp("��ǵ�����Դ�����뵽����ʱģ�Ϳ���")]
    public class ExportCGPrefab : MonoBehaviour
    {
        [Header("��Դͼ��")]
        public Texture2D m_CgIco;
        [Header("��Դ����")]
        public string m_CgName;
        [Header("��Դ����")]
        public string m_CgDescription;
        [Header("�ʲ�����")]
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
            return string.IsNullOrEmpty(m_CgName) ? "(δ�����Ķ���)" : m_CgName;
        }

        public string GetCgDescription()
        {
            return string.IsNullOrEmpty(m_CgDescription) ? "(��)" : m_CgDescription;
        }

        /// <summary>
        /// ���Ϊ�գ�����δ����
        /// </summary>
        /// <returns></returns>
        public string GetSafeCgType()
        {
            return string.IsNullOrEmpty(m_CgType) ? "δ����" : m_CgType;
        }
    }
}