using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace IOTLib
{
    public sealed class VariableDeclarations : IEnumerable<VariableDeclaration>
    {
        public VariableDeclarations()
        {
            collection = new VariableDeclarationCollection();
        }

        public VariableKind Kind;

        private VariableDeclarationCollection collection;

        internal Action OnVariableChanged;

        public object this[string variable]
        {
            get => Get(variable);
            set => Set(variable, value);
        }

        public void Set(string variable, object value)
        {
            if (string.IsNullOrEmpty(variable))
            {
                // Do nothing: safer for e.g. default event configurations,
                // where we don't specify a variable name. If we specified a default,
                // we might override a user value inadvertently.
                return;
            }

            if (collection.TryGetValue(variable, out var declaration))
            {
                if (declaration.value != value)
                {
                    declaration.value = value;
                    OnVariableChanged?.Invoke();
                }
            }
            else
            {
                collection.Add(new VariableDeclaration(variable, value));
                OnVariableChanged?.Invoke();
            }
        }

        public bool TryGet<T>(string variableName, out T? result)
        {
            result = default(T);

            Assert.IsFalse(string.IsNullOrEmpty(variableName), "��ȡ�ı������Ʋ���Ϊ��");

            if (IsDefined(variableName))
            {
                result = Get<T>(variableName);
                return true;
            }

            return false;
        }

        public object Get(string variable)
        {
            if (string.IsNullOrEmpty(variable))
            {
                throw new ArgumentException("No variable name specified.", nameof(variable));
            }

            if (collection.TryGetValue(variable, out var declaration))
            {
                return declaration.value;
            }

            throw new InvalidOperationException($"Variable not found: '{variable}'.");
        }

        public T Get<T>(string variable)
        {
            return (T)Get(variable, typeof(T));
        }

        public object Get(string variable, Type expectedType)
        {
            return ConversionUtility.Convert(Get(variable), expectedType);
        }

        public void Clear()
        {
            collection.Clear();
        }

        public bool IsDefined(string variable)
        {
            if (string.IsNullOrEmpty(variable))
            {
                throw new ArgumentException("No variable name specified.", nameof(variable));
            }

            return collection.Contains(variable);
        }

        public VariableDeclaration GetDeclaration(string variable)
        {
            if (collection.TryGetValue(variable, out var declaration))
            {
                return declaration;
            }

            throw new InvalidOperationException($"Variable not found: '{variable}'.");
        }

        public IEnumerator<VariableDeclaration> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)collection).GetEnumerator();
        }

        //ICloner ISpecifiesCloner.cloner => VariableDeclarationsCloner.instance;
    }
}
