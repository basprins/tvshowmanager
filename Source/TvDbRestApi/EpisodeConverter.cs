using System;
using Newtonsoft.Json;

namespace PerfectCode.TvDbRestApi
{
    public class EpisodeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize(reader, typeof(RestApiEpisode));
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof (IRestApiEpisode);
        }
    }
}