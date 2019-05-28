using System.Collections.Generic;

namespace PerfectCode.TvDbRestApi
{
    public interface IRestApiTVShowDetailedInfoResult
    {
        int Id { get; set; }
        string Name { get; set; }
        bool InProduction { get; set; }
        string Overview { get; set; }
        string FirstAirDate { get; set; }
        string LastAirDate { get; set; }
        string Status { get; set; }
        int NumberOfEpisodes { get; set; }
        int NumberOfSeasons { get; set; }
        List<IRestApiSeason> Seasons { get; set; }
        string BackdropPath { get; set; }
        string PosterPath { get; set; }
        string Homepage { get; set; }
    }
}