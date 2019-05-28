using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PerfectCode.Logging;
using PerfectCode.TimeMonitoring;

namespace PerfectCode.TvDbRestApi
{
    public class TVShowRestClient : ITVShowRestClient
    {
        private const string ApiKey = "83a7c74cb1b9e01f1f02b4e080901fec";

        private readonly ILogger _logger = new Logger(typeof (TVShowRestClient));

        private readonly HttpThrottlingClient _httpThrottlingClient;
        private IRestApiImagesConfiguration _imagesConfiguration;

        public TVShowRestClient()
        {
            _httpThrottlingClient = new HttpThrottlingClient(
                "https://api.themoviedb.org/3/",
                TimeSpan.FromSeconds(15));
        }

        public async Task<List<IRestApiTVShow>> GetTvShowsByName(string name)
        {
            using (new TimeMonitor("GetTVShowsByName"))
            {
                try
                {
                    var requestUri = $"search/tv?api_key={ApiKey}&query={name}";
                    var response = await _httpThrottlingClient.GetAsync(requestUri, "GetTvShowsByName").ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var results = JsonConvert.DeserializeObject<RestApiTVShowSearchResult>(content);

                        return new List<IRestApiTVShow>(results.Results);
                    }
                }
                catch (Exception e)
                {
                    _logger.Debug($"Exception: {e}");
                }

                return null;
            }
        }

        public async Task<RestApiTVShowSearchResult> DiscoverTVShows(
            DateTime minimumAirDate, 
            DateTime maximumAirDate, 
            TVShowDiscoveryType tvShowDiscoveryType = TVShowDiscoveryType.EpisodeAirDate, 
            int page = 1)
        {
            using (new TimeMonitor($"DiscoverTVShows [discovery type={tvShowDiscoveryType} from {minimumAirDate} to {maximumAirDate}"))
            {
                try
                {
                    var requestUri = $"discover/tv?page={page}&sort_by=first_air_date.desc&first_air_date.lte={DateTime.Now:YYYY-MM-DD}&api_key={ApiKey}";
                    var response = await _httpThrottlingClient.GetAsync(requestUri, "DiscoverTVShows");
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var tvShowDetails = JsonConvert.DeserializeObject<RestApiTVShowSearchResult>(content,
                            new JsonSerializerSettings
                            {
                                TypeNameHandling = TypeNameHandling.Objects
                            });

                        return tvShowDetails;
                    }

                    _logger.Error($"Response status not OK: {response.IsSuccessStatusCode}, {response.StatusCode}");
                }
                catch (Exception e)
                {
                    _logger.Debug($"Exception: {e}");
                }
            }

            return null;
        }

        public async Task<IRestApiTVShowDetailedInfoResult> GetDetailedTvShowInfo(int id)
        {
            using (new TimeMonitor($"GetDetailedTVShowInfo for id={id}"))
            {
                try
                {
                    var requestUri = $"tv/{id}?api_key={ApiKey}";
                    var response = await _httpThrottlingClient.GetAsync(requestUri, "GetDetailedTvShowInfo");
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var tvShowDetails = JsonConvert.DeserializeObject<RestApiTVShowDetailedInfoResult>(content,
                            new JsonSerializerSettings
                            {
                                TypeNameHandling = TypeNameHandling.Objects
                            });

                        return tvShowDetails;
                    }

                    _logger.Error($"Response status not OK: {response.IsSuccessStatusCode}, {response.StatusCode}");
                }
                catch (Exception e)
                {
                    _logger.Debug($"Exception: {e}");
                }

                return null;
            }
        }

        public async Task<IRestApiEpisodes> GetTvShowSeasonEpisodes(int tvShowId, int seasonNumber)
        {
            using (new TimeMonitor($"GetTVShowSeasonEpisodes for tvShow={tvShowId} and seasonNumber={seasonNumber}"))
            {
                try
                {
                    var requestUri = $"tv/{tvShowId}/season/{seasonNumber}?api_key={ApiKey}";
                    var response = await _httpThrottlingClient.GetAsync(requestUri, "GetTvShowSeasonEpisodes");
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<RestApiEpisodes>(content, new JsonSerializerSettings
                        {
                            Converters = new List<JsonConverter> { new EpisodeConverter() },
                            TypeNameHandling = TypeNameHandling.Objects
                        });

                        return result;
                    }
                }
                catch (Exception e)
                {
                    _logger.Debug($"Exception: {e}");
                }

                return null;
            }
        }

        public async Task<string> GetPosterImageUrl(string path, string sizeDefinition = "w342")
        {
            if (_imagesConfiguration == null)
            {
                _imagesConfiguration = await GetConfiguration();
            }
            return $"{_imagesConfiguration.BaseUrl}/{sizeDefinition}/{path}?api_key={ApiKey}";
        }

        public async Task<string> GetBackdropImageUrl(string path, string sizeDefinition = "w780")
        {
            if (_imagesConfiguration == null)
            {
                _imagesConfiguration = await GetConfiguration();
            }
            return $"{_imagesConfiguration.BaseUrl}/{sizeDefinition}/{path}?api_key={ApiKey}";
        }

        private async Task<IRestApiImagesConfiguration> GetConfiguration()
        {
            using (new TimeMonitor("GetConfiguration"))
            {
                try
                {
                    var requestUri = $"configuration?api_key={ApiKey}";
                    var response = await _httpThrottlingClient.GetAsync(requestUri, "GetConfiguration");
                    if (response.IsSuccessStatusCode)
                    {
                        var content = response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<RestApiConfiguration>(content.Result);
                        return result.Images;
                    }
                }
                catch (Exception e)
                {
                    _logger.Debug($"Exception: {e}");
                }

                return null;
            }
        }
    }

    public enum TVShowDiscoveryType
    {
        TVShowAirDate,
        EpisodeAirDate,
    }
}