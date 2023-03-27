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
        /// ʵ����һ��CGPrefab
        /// </summary>
        /// <param name="world_pos">����λ��</param>
        /// <param name="exportCG">����Assestģ�壬���ǳ����е�ʵ��������ʵ��ID�����</param>
        public static GameObject InstanceCGPrefab(Vector3 world_pos, ExportCGPrefab exportCG)
        {
            var newGo = GameObject.Instantiate<GameObject>(exportCG.gameObject, world_pos, Quaternion.identity);

            newGo.GetComponent<ExportCGPrefab>().SourceInstanceID = exportCG.gameObject.GetInstanceID();
            newGo.AddTag(TAGName);

            return newGo;
        }

        /// <summary>
        /// ��ȡһ�����������
        /// </summary>
        /// <param name="assest_id">�ʲ�ID</param>
        /// <param name="prefab">���ʵ��</param>
        /// <returns>�ҵ�True����֮</returns>
        public static bool TryGetCGPrefab(string assest_id, out ExportCGPrefab prefab)
        {
            if (CGPrefabs.TryGetValue(assest_id, out prefab))
            {
                return true;
            }

            Debug.LogError($"�Ҳ�����Դ:{assest_id}");
            return false;
        }
    }
}