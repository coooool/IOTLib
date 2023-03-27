using System;

namespace IOTLib
{
    public sealed class VariableDeclaration
    {
        public VariableDeclaration() { }

        public VariableDeclaration(string name, object value)
        {
            this.name = name;
            this.value = value;
        }

        public string name { get; private set; }

        public object value { get; set; }
    }
}
