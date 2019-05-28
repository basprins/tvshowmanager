using Newtonsoft.Json;

namespace PerfectCode.TvDbRestApi
{
    public class RestApiTVShow : IRestApiTVShow
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("backdrop_path")]
        public string BackdropPath { get; set; }

        [JsonProperty("original_name")]
        public string OriginalName { get; set; }

        [JsonProperty("first_air_date")]
        public string FirstAirDate { get; set; }

        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }

        [JsonProperty("popularity")]
        public double Popularity { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("overview")]
        public string Overview { get; set; }

        [JsonProperty("vote_average")]
        public double VoteAverage { get; set; }

        [JsonProperty("vote_count")]
        public int VoteCount { get; set; }

        public override string ToString()
        {
            return $"Id={Id}, Name={Name}, OriginalName={OriginalName}, FirstAirDate={FirstAirDate}";
        }

        public override bool Equals(object obj)
        {
            var result = false;
            var other = obj as RestApiTVShow;

            if (other != null)
            {
                result = Id.Equals(other.Id);
            }

            return result;
        }

        public override int GetHashCode()
        {
            // Bug in R#? shouldn't ask to make Id readonly, set via json
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return Id.GetHashCode();
        }
    }
}