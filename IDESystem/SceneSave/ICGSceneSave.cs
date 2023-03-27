using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IOTLib
{
    public interface ICGSceneEngine
    {
        /// <summary>
        /// �洢��������
        /// </summary>
        string Name { get; }

        /// <summary>
        /// ���泡��
        /// </summary>
        /// <param name="gameObject">��Ҫ���������</param>
        void SaveScene(IEnumerable<ExportCGPrefab> gameObjects);

        /// <summary>
        /// ��ȡ����
        /// </summary>
        void ReadScene(params string[] args);
    }
}