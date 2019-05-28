using System.Collections.Generic;
using Newtonsoft.Json;

namespace PerfectCode.TvDbRestApi
{
    public class RestApiTVShowSearchResult
    {
        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("results")]
        public List<RestApiTVShow> Results { get; set; }

        [JsonProperty("total_pages")]
        public int TotalPages { get; set; }

        [JsonProperty("total_results")]
        public int TotalResults { get; set; }

        public RestApiTVShowSearchResult()
        {
            Results = new List<RestApiTVShow>();
        }

        public override string ToString()
        {
            return $"Page={Page}, TotalPages={TotalPages}, TotalResults={Results.Count}";
        }
    }
}