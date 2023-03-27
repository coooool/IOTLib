using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IOTLib.Configure
{
    [System.Serializable]
    internal struct HostData
    {
        public string name { get; set; }
        public string value { get; set; }
    }
}
