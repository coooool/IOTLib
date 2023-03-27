using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace IOTLib
{
    internal interface ICGCustomEditor
    {
        Component target { get; }

        void OnGUI(Behaviour instance, IEnumerable<CGVar> vars);
    }
}
