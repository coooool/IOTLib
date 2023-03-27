using System;
using System.Collections.Generic;
using System.Text;

namespace IOTLib.IDESystem
{
    public interface ICGPrefabDragEvent
    {
        void OnDragUpdate();
        void OnDragComplete();
    }
}
