using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IOTLib
{
    public interface ICGSceneEngine
    {
        /// <summary>
        /// 存储引擎名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 保存场景
        /// </summary>
        /// <param name="gameObject">需要保存的物体</param>
        void SaveScene(IEnumerable<ExportCGPrefab> gameObjects);

        /// <summary>
        /// 读取场景
        /// </summary>
        void ReadScene(params string[] args);
    }
}