using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PerfectCode.TvDbRestApi
{
    public interface ITVShowRestClient
    {
        Task<List<IRestApiTVShow>> GetTvShowsByName(string name);
        Task<IRestApiTVShowDetailedInfoResult> GetDetailedTvShowInfo(int id);
        Task<string> GetPosterImageUrl(string path, string sizeDefinition = "w500");
        Task<string> GetBackdropImageUrl(string path, string sizeDefinition = "w342");

        Task<RestApiTVShowSearchResult> DiscoverTVShows(DateTime minimumAirDate, DateTime maximumAirDate, TVShowDiscoveryType tvShowDiscoveryType = TVShowDiscoveryType.EpisodeAirDate, int page=1);
        Task<IRestApiEpisodes> GetTvShowSeasonEpisodes(int tvShowId, int seasonNumber);
    }
}