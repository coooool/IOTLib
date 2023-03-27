using IOTLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IOTLib
{
    public class CGPrefabGroup
    {
        public bool Toggle { get; set; }
        public string GroupName { get; set; }
        public ExportCGPrefab[] Prefabs { get; set; }
    }
}