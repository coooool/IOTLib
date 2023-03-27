using System;
using UnityEngine;

namespace IOTLib.PointSystem
{
    public interface IPointProvider
    {
        Vector3 position { get; }
    }
}
