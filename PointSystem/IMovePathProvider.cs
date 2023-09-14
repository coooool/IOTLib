using System;
using System.Collections.Generic;
using UnityEngine;

namespace IOTLib
{
    public interface IMovePathProvider
    {
        void PlayPath(GameObject gameObject);
        void PausePath(GameObject gameObject);
        void StopPath(GameObject gameObject);
    }
}
