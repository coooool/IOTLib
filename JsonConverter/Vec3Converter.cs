using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace IOTLib
{
    public class Vec3Converter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Vector3);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if(reader.ValueType == typeof(string))
            {
                if(reader.Value is string vecStr)
                {
                    return vecStr.ToVector3();
                }
            }

            throw new InvalidOperationException("无法识别的Vec3类型");
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if(value != null)
            {
                var v3 = (Vector3)value;
                writer.WriteValue(v3.ToOriginStr());
            }
        }
    }
}
