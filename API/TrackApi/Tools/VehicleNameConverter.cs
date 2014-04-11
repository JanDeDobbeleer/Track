using System;
using Newtonsoft.Json;

namespace TrackApi.Tools
{
    public class VehicleNameConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return reader.Value.ToString().Remove(0,8).Trim();
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}
