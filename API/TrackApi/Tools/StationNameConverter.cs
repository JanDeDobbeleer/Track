using System;
using Newtonsoft.Json;

namespace TrackApi.Tools
{
    public class StationNameConverter: JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var index = reader.Value.ToString().IndexOf("[NMBS/SNCB]", StringComparison.Ordinal);
            var temp = reader.Value.ToString().Remove(index).Trim();
            return reader.Value.ToString().Remove(index).Trim();
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}
