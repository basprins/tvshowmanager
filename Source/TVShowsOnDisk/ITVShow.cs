using System;
using System.Collections.Generic;
using PerfectCode.TvDbRestApi;
using PerfectCode.TVShowManager.Application;

namespace PerfectCode.TVShows
{
    public interface ITVShow
    {
        string Path { get; }
        string Name { get; set; }
        int? WebID { get; set; }

        List<ISeason> Seasons { get; }
        bool ComingSoon { get; }
        bool DownloadsAvailable { get; }
        bool HasNewEpisodes { get; }
        string PosterImageUrl { get; }
        string BackdropUrl { get; }
        bool InProduction { get; }
        string Homepage { get; set; }
        string Status { get; set; }
        bool Updating { get; set; }
        event EventHandler PropertyChanged;
        void CheckForUpdates(ITVShowRestClient restApiClient);
        void UpdateSeason(ITVShowRestClient restApiClient, ISeason season);
        void CheckFileSystem(ISeasonFactory seasonsFactory);
        void FirePropertyChanged();
    }
}