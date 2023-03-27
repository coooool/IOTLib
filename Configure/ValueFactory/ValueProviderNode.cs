using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IOTLib.Configure.ValueFactory
{
    public struct ValueProviderNode
    {
        public string Name { get; set; }
        public IConfigureValueProvider ValueProvider { get; set; }
    }
}
