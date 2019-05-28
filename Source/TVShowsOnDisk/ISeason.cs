using System;
using System.Collections.Generic;
using PerfectCode.TvDbRestApi;

namespace PerfectCode.TVShows
{
    public interface ISeason
    {
        event EventHandler PropertyChanged;

        string Path { get; }
        int Number { get; }
        List<IEpisode> Episodes { get; }
        ITVShow TVShow { get; set; }
        bool Updating { get; set; }
        void Update(IRestApiSeason detailedTVShowInfo, ITVShowRestClient restApiClient);
        void FirePropertyChanged();
    }
}