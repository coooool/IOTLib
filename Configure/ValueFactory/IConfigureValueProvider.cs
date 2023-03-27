using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IOTLib.Configure.ValueFactory
{
    public interface IConfigureValueProvider
    {
        string GetConfigureValue(string key);
    }
}
