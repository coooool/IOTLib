using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace IOTLib
{
    public static class ConversionUtility
    {
        public enum ConversionType
        {
            Impossible,
            Identity,
            Upcast,
            Downcast,
            NumericImplicit,
            NumericExplicit,
            UnityHierarchy,
            ToString
        }

        private static readonly Dictionary<ConversionQuery, ConversionType> conversionTypesCache = new Dictionary<ConversionQuery, ConversionType>(new ConversionQueryComparer());

        private static bool RespectsIdentity(Type source, Type destination)
        {
            return source == destination;
        }

        private static bool IsUpcast(Type source, Type destination)
        {
            return destination.IsAssignableFrom(source);
        }

        private static bool IsDowncast(Type source, Type destination)
        {
            return source.IsAssignableFrom(destination);
        }

        public static bool HasImplicitNumericConversion(Type source, Type destination)
        {
            return implicitNumericConversions.ContainsKey(source) && implicitNumericConversions[source].Contains(destination);
        }

        public static bool HasExplicitNumericConversion(Type source, Type destination)
        {
            return explicitNumericConversions.ContainsKey(source) && explicitNumericConversions[source].Contains(destination);
        }

        public static bool HasNumericConversion(Type source, Type destination)
        {
            return HasImplicitNumericConversion(source, destination) || HasExplicitNumericConversion(source, destination);
        }

        private static object NumericConversion(object value, Type type)
        {
            return System.Convert.ChangeType(value, type);
        }

        private static object UnityHierarchyConversion(object value, Type type)
        {
            if (value.IsUnityNull())
            {
                return null;
            }

            if (type == typeof(GameObject) && value is Component)
            {
                return ((Component)value).gameObject;
            }
            else if (typeof(Component).IsAssignableFrom(type) || type.IsInterface)
            {
                if (value is Component)
                {
                    return ((Component)value).GetComponent(type);
                }
                else if (value is GameObject)
                {
                    return ((GameObject)value).GetComponent(type);
                }
            }

            throw new System.InvalidCastException();
        }

        public static ConversionType GetRequiredConversion(object value, Type type)
        {
            return GetRequiredConversion(value.GetType(), type);
        }

        private static bool HasUnityHierarchyConversion(Type source, Type destination)
        {
            if (destination == typeof(GameObject))
            {
                return typeof(Component).IsAssignableFrom(source);
            }
            else if (typeof(Component).IsAssignableFrom(destination) || destination.IsInterface)
            {
                return source == typeof(GameObject) || typeof(Component).IsAssignableFrom(source);
            }

            return false;
        }


        private static ConversionType DetermineConversionType(ConversionQuery query)
        {
            var source = query.source;
            var destination = query.destination;

            if (source == null)
            {
                if (destination.IsNullable())
                {
                    return ConversionType.Identity;
                }
                else
                {
                    return ConversionType.Impossible;
                }
            }

            if (RespectsIdentity(source, destination))
            {
                return ConversionType.Identity;
            }
            else if (IsUpcast(source, destination))
            {
                return ConversionType.Upcast;
            }
            else if (IsDowncast(source, destination))
            {
                return ConversionType.Downcast;
            }
            // Disabling *.ToString conversion, because it's more often than otherwise very confusing
            /*else if (ExpectsString(source, destination))
            {
                return ConversionType.ToString;
            }*/
            else if (HasImplicitNumericConversion(source, destination))
            {
                return ConversionType.NumericImplicit;
            }
            else if (HasExplicitNumericConversion(source, destination))
            {
                return ConversionType.NumericExplicit;
            }
            else if (HasUnityHierarchyConversion(source, destination))
            {
                return ConversionType.UnityHierarchy;
            }

            return ConversionType.Impossible;
        }


        public static ConversionType GetRequiredConversion(Type source, Type destination)
        {
            var query = new ConversionQuery(source, destination);

            if (!conversionTypesCache.TryGetValue(query, out var conversionType))
            {
                conversionType = DetermineConversionType(query);
                conversionTypesCache.Add(query, conversionType);
            }

            return conversionType;
        }


        public static object Convert(object value, Type type)
        {
            return Convert(value, type, GetRequiredConversion(value, type));
        }

        private static object Convert(object value, Type type, ConversionType conversionType)
        {
            if (conversionType == ConversionType.Impossible)
            {
                throw new System.InvalidCastException($"Cannot convert from '{value?.GetType().ToString() ?? "null"}' to '{type}'.");
            }

            try
            {
                switch (conversionType)
                {
                    case ConversionType.Identity:
                    case ConversionType.Upcast:
                    case ConversionType.Downcast:
                        return value;

                    case ConversionType.ToString:
                        return value.ToString();

                    case ConversionType.NumericImplicit:
                    case ConversionType.NumericExplicit:
                        return NumericConversion(value, type);

                    case ConversionType.UnityHierarchy:
                        return UnityHierarchyConversion(value, type);

                    default:
                        throw new InvalidCastException($"无法转换的类型:{conversionType.GetType().Name}");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidCastException($"Failed to convert from '{value?.GetType().ToString() ?? "null"}' to '{type}' via {conversionType}.", ex);
            }
        }

        private struct ConversionQuery : IEquatable<ConversionQuery>
        {
            public readonly Type source;
            public readonly Type destination;

            public ConversionQuery(Type source, Type destination)
            {
                this.source = source;
                this.destination = destination;
            }

            public bool Equals(ConversionQuery other)
            {
                return
                    source == other.source &&
                    destination == other.destination;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is ConversionQuery))
                {
                    return false;
                }

                return Equals((ConversionQuery)obj);
            }

            public override int GetHashCode()
            {
                return HashUtility.GetHashCode(source, destination);
            }
        }

        // Make sure the equality comparer doesn't use boxing
        private struct ConversionQueryComparer : IEqualityComparer<ConversionQuery>
        {
            public bool Equals(ConversionQuery x, ConversionQuery y)
            {
                return x.Equals(y);
            }

            public int GetHashCode(ConversionQuery obj)
            {
                return obj.GetHashCode();
            }
        }

        #region Numeric Conversions

        // https://msdn.microsoft.com/en-us/library/y5b434w4.aspx
        private static readonly Dictionary<Type, HashSet<Type>> implicitNumericConversions = new Dictionary<Type, HashSet<Type>>()
        {
            {
                typeof(sbyte),
                new HashSet<Type>()
                {
                    typeof(byte),
                    typeof(int),
                    typeof(long),
                    typeof(float),
                    typeof(double),
                    typeof(decimal)
                }
            },
            {
                typeof(byte),
                new HashSet<Type>()
                {
                    typeof(short),
                    typeof(ushort),
                    typeof(int),
                    typeof(uint),
                    typeof(long),
                    typeof(ulong),
                    typeof(float),
                    typeof(double),
                    typeof(decimal)
                }
            },
            {
                typeof(short),
                new HashSet<Type>()
                {
                    typeof(int),
                    typeof(long),
                    typeof(float),
                    typeof(double),
                    typeof(decimal)
                }
            },
            {
                typeof(ushort),
                new HashSet<Type>()
                {
                    typeof(int),
                    typeof(uint),
                    typeof(long),
                    typeof(ulong),
                    typeof(float),
                    typeof(double),
                    typeof(decimal),
                }
            },
            {
                typeof(int),
                new HashSet<Type>()
                {
                    typeof(long),
                    typeof(float),
                    typeof(double),
                    typeof(decimal)
                }
            },
            {
                typeof(uint),
                new HashSet<Type>()
                {
                    typeof(long),
                    typeof(ulong),
                    typeof(float),
                    typeof(double),
                    typeof(decimal)
                }
            },
            {
                typeof(long),
                new HashSet<Type>()
                {
                    typeof(float),
                    typeof(double),
                    typeof(decimal)
                }
            },
            {
                typeof(char),
                new HashSet<Type>()
                {
                    typeof(ushort),
                    typeof(int),
                    typeof(uint),
                    typeof(long),
                    typeof(ulong),
                    typeof(float),
                    typeof(double),
                    typeof(decimal)
                }
            },
            {
                typeof(float),
                new HashSet<Type>()
                {
                    typeof(double)
                }
            },
            {
                typeof(ulong),
                new HashSet<Type>()
                {
                    typeof(float),
                    typeof(double),
                    typeof(decimal)
                }
            },
        };

        // https://msdn.microsoft.com/en-us/library/yht2cx7b.aspx
        private static readonly Dictionary<Type, HashSet<Type>> explicitNumericConversions = new Dictionary<Type, HashSet<Type>>()
        {
            {
                typeof(sbyte),
                new HashSet<Type>()
                {
                    typeof(byte),
                    typeof(ushort),
                    typeof(uint),
                    typeof(ulong),
                    typeof(char)
                }
            },
            {
                typeof(byte),
                new HashSet<Type>()
                {
                    typeof(sbyte),
                    typeof(char)
                }
            },
            {
                typeof(short),
                new HashSet<Type>()
                {
                    typeof(sbyte),
                    typeof(byte),
                    typeof(ushort),
                    typeof(uint),
                    typeof(ulong),
                    typeof(char)
                }
            },
            {
                typeof(ushort),
                new HashSet<Type>()
                {
                    typeof(sbyte),
                    typeof(byte),
                    typeof(short),
                    typeof(char)
                }
            },
            {
                typeof(int),
                new HashSet<Type>()
                {
                    typeof(sbyte),
                    typeof(byte),
                    typeof(short),
                    typeof(ushort),
                    typeof(uint),
                    typeof(ulong),
                    typeof(char)
                }
            },
            {
                typeof(uint),
                new HashSet<Type>()
                {
                    typeof(sbyte),
                    typeof(byte),
                    typeof(short),
                    typeof(ushort),
                    typeof(int),
                    typeof(char)
                }
            },
            {
                typeof(long),
                new HashSet<Type>()
                {
                    typeof(sbyte),
                    typeof(byte),
                    typeof(short),
                    typeof(ushort),
                    typeof(int),
                    typeof(uint),
                    typeof(ulong),
                    typeof(char)
                }
            },
            {
                typeof(ulong),
                new HashSet<Type>()
                {
                    typeof(sbyte),
                    typeof(byte),
                    typeof(short),
                    typeof(ushort),
                    typeof(int),
                    typeof(uint),
                    typeof(long),
                    typeof(char)
                }
            },
            {
                typeof(char),
                new HashSet<Type>()
                {
                    typeof(sbyte),
                    typeof(byte),
                    typeof(short)
                }
            },
            {
                typeof(float),
                new HashSet<Type>()
                {
                    typeof(sbyte),
                    typeof(byte),
                    typeof(short),
                    typeof(ushort),
                    typeof(int),
                    typeof(uint),
                    typeof(long),
                    typeof(ulong),
                    typeof(char),
                    typeof(decimal)
                }
            },
            {
                typeof(double),
                new HashSet<Type>()
                {
                    typeof(sbyte),
                    typeof(byte),
                    typeof(short),
                    typeof(ushort),
                    typeof(int),
                    typeof(uint),
                    typeof(long),
                    typeof(ulong),
                    typeof(char),
                    typeof(float),
                    typeof(decimal),
                }
            },
            {
                typeof(decimal),
                new HashSet<Type>()
                {
                    typeof(sbyte),
                    typeof(byte),
                    typeof(short),
                    typeof(ushort),
                    typeof(int),
                    typeof(uint),
                    typeof(long),
                    typeof(ulong),
                    typeof(char),
                    typeof(float),
                    typeof(double)
                }
            }
        };

        #endregion
    }
}
