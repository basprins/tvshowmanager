using System.Collections.Generic;
using Newtonsoft.Json;

namespace PerfectCode.TvDbRestApi
{
    public class RestApiTVShowDetailedInfoResult : IRestApiTVShowDetailedInfoResult
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("in_production")]
        public bool InProduction { get; set; }

        [JsonProperty("overview")]
        public string Overview { get; set; }

        [JsonProperty("first_air_date")]
        public string FirstAirDate { get; set; }

        [JsonProperty("last_air_date")]
        public string LastAirDate { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("backdrop_path")]
        public string BackdropPath { get; set; }

        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }

        [JsonProperty("homepage")]
        public string Homepage { get; set; }

        [JsonProperty("number_of_episodes")]
        public int NumberOfEpisodes { get; set; }

        [JsonProperty("number_of_seasons")]
        public int NumberOfSeasons { get; set; }

        [JsonProperty("seasons")]
        public List<IRestApiSeason> Seasons { get; set; }

        [JsonConstructor]
        public RestApiTVShowDetailedInfoResult(List<RestApiSeason> seasons)
        {
            Seasons = new List<IRestApiSeason>();

            foreach (var season in seasons)
            {
                Seasons.Add(season);
            }
        }

        public override string ToString()
        {
            return $"Id={Id}, Name={Name}, Seasons={Seasons.Count}";
        }
    }
}
