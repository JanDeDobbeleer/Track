using System;
using Newtonsoft.Json;

namespace TrackApi.Tools
{
    public class PlatformConverter:JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return (string.IsNullOrWhiteSpace(reader.Value.ToString()))?"?":reader.Value.ToString();
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}
