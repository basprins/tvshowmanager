using System.Collections.Generic;
using Newtonsoft.Json;

namespace PerfectCode.TvDbRestApi
{
    public class RestApiConfiguration
    {
        [JsonProperty("images")]
        public RestApiImagesConfiguration Images { get; set; }
    }

    public class RestApiImagesConfiguration : IRestApiImagesConfiguration
    {
        [JsonProperty("base_url")]
        public string BaseUrl { get; set; }

        [JsonProperty("backdrop_sizes")]
        public List<string> BackdropSizes { get; set; }

        [JsonProperty("logo_sizes")]
        public List<string> LogoSizes { get; set; }

        [JsonProperty("poster_sizes")]
        public List<string> PosterSizes { get; set; }
    }
}