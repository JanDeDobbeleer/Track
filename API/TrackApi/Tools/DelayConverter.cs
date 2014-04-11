using System;
using Newtonsoft.Json;

namespace TrackApi.Tools
{
    public class DelayConverter:JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            int delay;
            if (!int.TryParse(reader.Value.ToString(), out delay)) 
                return "0";
            if(delay == 0)
                return "0";
            var minutes = delay/60;
            return "+ " + minutes + "'";
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}
