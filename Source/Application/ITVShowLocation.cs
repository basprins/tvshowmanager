using System.Collections.Generic;
using PerfectCode.FileSystemIO;
using PerfectCode.TVShowManager.Application;

namespace PerfectCode.TVShows
{
    public interface ITVShowLocation
    {
        List<ITVShow> Locate(string path, ITVShowFactory tvShowFactory, IFileSystem fileSystem);
    }
}