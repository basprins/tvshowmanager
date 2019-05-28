using System;
using PerfectCode.TvDbRestApi;

namespace PerfectCode.TVShows
{
    public interface IEpisode
    {
        event EventHandler PropertyChanged;

        int Number { get; }
        ISeason Season { get; }
        string Name { get; set; }
        DateTime AirDate { get; set; }
        bool IsPresentOnDisk { get; }
        bool CanDownload { get; }
        bool ComingSoon { get; }
        bool IsNew { get; }
        string EpisodeFullName { get; set; }
        void Update(IRestApiEpisode episodeInfo);
        void OpenInExplorer();
        void MarkAsWatched();
        void MarkAsUnwatched();
    }
}