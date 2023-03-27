using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IOTLib
{

    /// <summary>
    /// JSON 存储引擎的数据，使用UNITY原生JSON序列化工具，速度极快GC少。
    /// </summary>
    [Serializable]
    public struct CGModelJsonData
    {
        public string Id;
        public CGModelComponentJsonData[] Data;

        public string Name;
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;
    }

    [Serializable]
    public struct CGModelComponentJsonData
    {
        [SerializeField]
        public string FileId;
        [SerializeField]
        public string Data;
    }
}