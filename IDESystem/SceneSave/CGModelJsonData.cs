using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IOTLib
{

    /// <summary>
    /// JSON �洢��������ݣ�ʹ��UNITYԭ��JSON���л����ߣ��ٶȼ���GC�١�
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