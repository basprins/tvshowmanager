using System.Collections.Generic;
using Newtonsoft.Json;

namespace PerfectCode.TvDbRestApi
{
    public class RestApiSeason : IRestApiSeason
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("season_number")]
        public int Number { get; set; }

        [JsonProperty("air_date")]
        public string AirDate { get; set; }

        [JsonProperty("episodes")]
        public List<IRestApiEpisode> Episodes { get; }

        public RestApiSeason()
        {
            Episodes = new List<IRestApiEpisode>();
        }

        public override string ToString()
        {
            return $"Id={Id}, Number={Number}, Episodes={Episodes.Count}";
        }
    }
}