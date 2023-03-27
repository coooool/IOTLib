using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace IOTLib.IDESystem
{
    public struct ShortcutKeyData
    {
        public bool Control { get; set; }
        public bool Alt { get; set; }
        public short Shift { get; set; }

        public KeyCode KeyCode { get; set; }
        public ushort MouseButton { get; set; } 

        public UnityAction OnTrigger { get; set; }
    }
}
