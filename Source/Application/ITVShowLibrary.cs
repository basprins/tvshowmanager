using System;
using System.Collections.Generic;
using PerfectCode.TVShows;

namespace PerfectCode.TVShowManager
{
    public delegate void TVShowAddedEvent(object sender, TVShowEventArgs args);
    public delegate void TVShowRemovedEvent(object sender, TVShowEventArgs args);
    public delegate void TVShowUpdatedEvent(object sender, TVShowEventArgs args);
    public delegate void TVShowLibraryUpdateStartedEvent(object sender, EventArgs args);
    public delegate void TVShowProgressChangedEvent(object sender, UpdateTVShowProgressEvent args);

    public interface ITVShowLibrary
    {
        event TVShowAddedEvent TVShowAdded;
        event TVShowRemovedEvent TVShowRemoved;
        event TVShowProgressChangedEvent ProgressChanged;
        event TVShowLibraryUpdateStartedEvent TVShowLibraryUpdateStarted;

        List<ITVShow> TVShows { get; }
        string ImportFolder { get; set; }
        string TVShowLocationPath { get; set; }

        void Add(ITVShow tvShowOnDisk);
        void AddMany(List<ITVShow> tvShows);
        void RemoveTVShow(ITVShow tvShow);
        void Store();
        void UpdateTvShow(ITVShow tvShow);
        void UpdateSeason(ITVShow show, ISeason season);
    }
}