using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UMOD;

namespace IOTLib
{
    [BindSystem(typeof(TagSystem))]
    public class DynGameTagAgent : DataBehaviour
    {
        [UnityEngine.SerializeField]
        internal string[] m_subTag = new string[0];

        /// <summary>
        /// 是否存在指定的子标签
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool HasTag(string tag)
        {
            return m_subTag.Where(p => p == tag).Any();
        }

        /// <summary>
        /// 设置标签,这里会删除所有旧的子标签并使用新的标签替代
        /// </summary>
        /// <param name="mainTag">主标签</param>
        /// <param name="subTags">子标签</param>
        public void SetTag(params string[] tags)
        {
            RemoveAllTag();
            AddTagRange(tags);
        }

        /// <summary>
        /// 添加子标签成功返回True，否则可能存在同名的
        /// </summary>
        /// <param name="tag">目标标签</param>
        /// <returns></returns>
        public bool Add(string tag)
        {
            if(HasTag(tag))
            {
                return false;
            }

            do
            {
                for (var i = m_subTag.Length - 1; i >= 0; i--)
                {
                    if (string.IsNullOrEmpty(m_subTag[i]))
                    {
                        m_subTag[i] = tag;
                        return true;
                    }
                }

                Array.Resize(ref m_subTag, m_subTag.Length + 5);
            } while (true);
        }

        /// <summary>
        /// 删除所有子标签
        /// </summary>
        public void RemoveAllTag()
        {
            Array.Resize(ref m_subTag, 0);
        }

        /// <summary>
        /// 批量添加子标签，会自动去重
        /// </summary>
        /// <param name="tags"></param>
        public void AddTagRange(params string[] tags)
        {
            foreach(var tag in tags)
            {
                Add(tag);
            }
        }

        /// <summary>
        /// 删除子标签，成功返回True，没有目标返回False
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool RemoveTag(string tag)
        {
            for (var i = m_subTag.Length - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(m_subTag[i]))
                {
                    m_subTag[i] = string.Empty;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 比较标签
        /// </summary>
        /// <param name="tags">标签</param>
        /// <param name="anyTag">任意标签命中即可</param>
        /// <returns></returns>
        public bool CompareTag(bool anyTag = true, params string[] tags)
        {
            var findCount = 0;

            foreach(string tag in tags)
            {
                if (true == HasTag(tag))
                {
                    findCount++;
                    if (anyTag)
                        break;
                }
            }

            //if (false == string.IsNullOrEmpty(mainTag))
            //{
            //    if (m_mainTag != mainTag) return false;
            //    else findCount++;
            //}

            if (findCount == 0) return false;

            return true;
        }
    }
}
