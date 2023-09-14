using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace IOTLib
{
    public interface IPointPathProvider
    {
        Vector3[] points { get; }
    }
}
