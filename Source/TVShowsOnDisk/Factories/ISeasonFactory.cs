using System.Collections.Generic;
using PerfectCode.TVShows;

namespace PerfectCode.TVShowManager.Application
{
    public interface ISeasonFactory
    {
        List<ISeason> Create(ITVShow tvShow);
    }
}