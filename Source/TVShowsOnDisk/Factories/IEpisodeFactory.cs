using System.Collections.Generic;
using PerfectCode.FileSystemIO;
using PerfectCode.TVShows;

namespace PerfectCode.TVShowManager.Application
{
    public interface IEpisodeFactory
    {
        List<IEpisode> Create(ISeason season, string seasonFullPath, IFileSystem fileSystem);
    }
}