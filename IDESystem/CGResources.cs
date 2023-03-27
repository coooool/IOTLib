using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IOTLib
{
    public static class CGResources
    {
        public const string TAGName = "CGPrefab";

        static Dictionary<string, ExportCGPrefab> CGPrefabs;

        static CGResources()
        {
            CGPrefabs = new Dictionary<string, ExportCGPrefab>();
        }


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            var allItem = Resources.LoadAll<ExportCGPrefab>("");
            foreach (var item in allItem)
            {
                CGPrefabs[item.m_PrefabID] = item;
            }
        }

        /// <summary>
        /// 实例化一个CGPrefab
        /// </summary>
        /// <param name="world_pos">世界位置</param>
        /// <param name="exportCG">来自Assest模板，不是场景中的实例，否则实例ID会错误</param>
        public static GameObject InstanceCGPrefab(Vector3 world_pos, ExportCGPrefab exportCG)
        {
            var newGo = GameObject.Instantiate<GameObject>(exportCG.gameObject, world_pos, Quaternion.identity);

            newGo.GetComponent<ExportCGPrefab>().SourceInstanceID = exportCG.gameObject.GetInstanceID();
            newGo.AddTag(TAGName);

            return newGo;
        }

        /// <summary>
        /// 获取一个导出的组件
        /// </summary>
        /// <param name="assest_id">资产ID</param>
        /// <param name="prefab">输出实例</param>
        /// <returns>找到True，反之</returns>
        public static bool TryGetCGPrefab(string assest_id, out ExportCGPrefab prefab)
        {
            if (CGPrefabs.TryGetValue(assest_id, out prefab))
            {
                return true;
            }

            Debug.LogError($"找不到资源:{assest_id}");
            return false;
        }
    }
}