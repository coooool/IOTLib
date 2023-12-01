using System;
using System.Collections.Generic;
using UMOD;
using System.Linq;
using System.Collections;
using UnityEngine;

namespace IOTLib
{
    [GameSystem]
    [SystemDescribe(Author = "吴", 
        Dependent = "无", 
        Describe = "GameObject动态多Tag查找系统，独立于Unity的GameObject(Tag)系统",
        Name = "GameObject标记系统", 
        Version = "0.2")]
    public class TagSystem : SingleBaseSystem<TagSystem> /*, IEnumerable<DynGameTagAgent> */
    {
        /// <summary>
        /// 查找一批Tag
        /// </summary>
        /// <param name="childTags">二级标签</param>
        /// <param name="anyTag">任意标签命中即可</param>
        /// <returns>如果没有找到返回null</returns>
        public static List<GameObject> Find(bool anyTag = true, params string[] childTags)
        {
            var result = new List<GameObject>();

            foreach(DynGameTagAgent a in Install)
            {
                if(a.CompareTag(anyTag, childTags))
                {
                    result.Add(a.gameObject);
                }
            }

            return result;
        }

        /// <summary>
        /// 查找一批Tag并返回挂载的指定类型的Unity组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="anyTag">任意子标签命中即可</param>
        /// <param name="childTags">次标签</param>
        /// <returns>T类型的Component</returns>
        public static List<T> Find<T>(bool anyTag = true, params string[] childTags) where T: Component
        {
            var datas = new List<T>();
            var find_result = Find(anyTag, childTags);

            if (find_result.Count == 0) return datas; 

            foreach(var a in find_result)
            {
                if(a.TryGetComponent<T>(out var b))
                {
                    datas.Add(b);
                }
            }

            //if (datas.Count == 0)
            //    Debug.LogWarning($"查找Tag组件{typeof(T).Name}为空,标签:{string.Join(',', childTags)}");

            return datas;
        }

        /// <summary>
        /// 查找一个单一标记
        /// </summary>
        /// <param name="subTag">次标签</param>
        /// <param name="childTags">任意子标签命中即可</param>
        /// <returns>如果没有找到返回null</returns>
        public static GameObject? FindSingle(bool childTags = true, params string[] subTag)
        {
            foreach (DynGameTagAgent a in Install)
            {
                if(a.CompareTag(childTags, subTag))
                {
                    return a.gameObject;
                }
            }

            //Debug.LogWarning($"查找Tag为空,次标签:{string.Join(',', subTag)}");

            return null;
        }

        /// <summary>
        /// 查找一个单一标记并返回挂载的指定类型组件
        /// </summary>
        /// <param name="childTags">次标签</param>
        /// <param name="anyTag">任意子标签命中即可</param>
        /// <returns>如果没有找到返回null</returns>
        public static T? FindSingle<T>(bool anyTag = true, params string[] childTags) where T: Component
        {
            var find_result = FindSingle(anyTag, childTags);

            if(find_result == null)
            {
                return null;
            }

            if(find_result.TryGetComponent<T>(out var b))
            {
                return b;
            }
            else
            {
                throw new InvalidOperationException($"在目标实例上{find_result.gameObject.name}获取组件{typeof(T).Name}不存在,标签:{string.Join(',', childTags)}");
            }
        }
    }
}
