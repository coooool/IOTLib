using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace IOTLib
{
    public static class TypeUtility
    {
        private static readonly HashSet<Type> _numericTypes = new HashSet<Type>
        {
            typeof(byte),
            typeof(sbyte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal)
        };

        private static readonly HashSet<Type> _numericConstructTypes = new HashSet<Type>
        {
            typeof(Vector2),
            typeof(Vector3),
            typeof(Vector4),
            typeof(Quaternion),
            typeof(Matrix4x4),
            typeof(Rect),
        };

        private static readonly HashSet<Type> typesWithShortStrings = new HashSet<Type>()
        {
            typeof(string),
            typeof(Vector2),
            typeof(Vector3),
            typeof(Vector4)
        };


        public static bool IsNullable(this Type type)
        {
            // http://stackoverflow.com/a/1770232
            return type.IsReferenceType() || Nullable.GetUnderlyingType(type) != null;
        }

        public static bool IsReferenceType(this Type type)
        {
            return !type.IsValueType;
        }
    }
}
