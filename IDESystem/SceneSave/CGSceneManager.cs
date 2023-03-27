using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IOTLib
{
    /// <summary>
    /// CG����������,��ȡ�뱣��
    /// </summary>
    public static class CGSceneManager
    {
        readonly static List<ICGSceneEngine> sceneEngines;

        static CGSceneManager()
        {
            sceneEngines = new List<ICGSceneEngine>()
        {
            new CGSceneJsonEngine(),
        };
        }

        public static void RegisterEngine(ICGSceneEngine engine)
        {
            if (sceneEngines.FindIndex(p => p.Name == engine.Name) == -1)
            {
                sceneEngines.Add(engine);
            }
            else
            {
                Debug.LogError($"Ŀ�������Ѿ�����:{engine.Name}");
            }
        }

        public static bool RemoveEngine(string engine_name)
        {
            var idx = sceneEngines.FindIndex(p => p.Name == engine_name);

            if (idx != -1)
            {
                sceneEngines.RemoveAt(idx);
                return true;
            }

            return false;
        }

        static ICGSceneEngine FindEngine(string engine_name)
        {
            var engine = sceneEngines.Find(p => p.Name == engine_name);
            if (engine == null)
            {
                throw new InvalidOperationException($"�Ҳ����洢����:{engine_name}");
            }

            return engine;
        }

        /// <summary>
        /// ���泡��
        /// </summary>
        /// <param name="engine_name"></param>
        /// <param name="prefabType"></param>
        public static void Save(string engine_name, params string[] prefabType)
        {
            var model = TagSystem.Find<ExportCGPrefab>(false, prefabType);
            FindEngine(engine_name).SaveScene(model);
        }

        /// <summary>
        /// ��ȡ����
        /// </summary>
        /// <param name="engine_name"></param>
        /// <param name="args"></param>
        public static void LoadScene(string engine_name, params string[] args)
        {
            FindEngine(engine_name).ReadScene(args);
        }
    }
}