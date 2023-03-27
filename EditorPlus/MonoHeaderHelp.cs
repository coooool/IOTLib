using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace IOTLib
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MonoHeaderHelp :　Attribute
    {
        public string Text { get; set; }

        public MonoHeaderHelp(string text)
        {
            Text = text;
        }
    }
}
