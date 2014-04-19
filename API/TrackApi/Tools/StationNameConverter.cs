using System;
using System.Net;
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
            if (!reader.Value.ToString().Contains("[NMBS/SNCB]"))
                return HttpUtility.HtmlDecode(reader.Value.ToString());
            var index = reader.Value.ToString().IndexOf("[NMBS/SNCB]", StringComparison.Ordinal);
            return HttpUtility.HtmlDecode(reader.Value.ToString().Remove(index).Trim());
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}
