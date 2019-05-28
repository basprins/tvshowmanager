using System.Collections.Generic;
using Newtonsoft.Json;

namespace PerfectCode.TvDbRestApi
{
    public class RestApiEpisodes : IRestApiEpisodes
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("episodes")]
        public List<IRestApiEpisode> Episodes { get; set; }

        public RestApiEpisodes()
        {
            Episodes = new List<IRestApiEpisode>();
        }

        public override string ToString()
        {
            return $"Id={Id}, Episodes={Episodes.Count}";
        }
    }
}