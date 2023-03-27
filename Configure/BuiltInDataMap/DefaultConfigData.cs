using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IOTLib.Configure
{
    [System.Serializable]
    internal struct DefaultConfigData
    {
        public ApiRequestData[] apimap;
        public HostData[] hosts;
    }
}
