using System;
using UnityEngine;

namespace IOTLib
{
    public interface IPointProvider
    {
        Vector3 position { get; }
    }
}
