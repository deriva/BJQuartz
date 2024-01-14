using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BJ.Quartz.Utilities
{
    /// <summary>
    /// Json序列化时null转换为空字符串
    /// </summary>
    public class NullToStringEmptyConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(string).Equals(objectType);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            JToken jt = JToken.ReadFrom(reader);
            return jt.Value<string>();
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value == null ? string.Empty : value.ToString());
        }
    }
}
