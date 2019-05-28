using System;
using PerfectCode.TVShows;

namespace PerfectCode.TVShowManager
{
    public class UpdateTVShowProgressEvent : EventArgs
    {
        public ITVShow TVShow { get; }
        public bool FinishedUpdatingTVShow { get; }

        public UpdateTVShowProgressEvent(ITVShow tvShow, bool finishedUpdatingTVShow)
        {
            TVShow = tvShow;
            FinishedUpdatingTVShow = finishedUpdatingTVShow;
        }
    }
}