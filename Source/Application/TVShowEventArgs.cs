using System;
using PerfectCode.TVShows;

namespace PerfectCode.TVShowManager
{
    public class TVShowEventArgs : EventArgs
    {
        public ITVShow TVShow { get; }

        public TVShowEventArgs(ITVShow tvShow)
        {
            TVShow = tvShow;
        }
    }
}