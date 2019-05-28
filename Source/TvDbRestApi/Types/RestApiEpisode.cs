using Newtonsoft.Json;

namespace PerfectCode.TvDbRestApi
{
    public class RestApiEpisode : IRestApiEpisode
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("episode_number")]
        public int Number { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("air_date")]
        public string AirDate { get; set; }

        [JsonProperty("overview")]
        public string Overview { get; set; }

        public override string ToString()
        {
            return $"Id={Id}, Number={Number}, Name={Name}";
        }
    }
}