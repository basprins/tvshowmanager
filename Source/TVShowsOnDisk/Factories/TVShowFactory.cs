using PerfectCode.FileSystemIO;
using PerfectCode.TVShows;

namespace PerfectCode.TVShowManager.Application
{
    public class TVShowFactory : ITVShowFactory
    {
        public ITVShow Create(string tvShowFullPath, IFileSystem fileSystem)
        {
            return new TVShow(tvShowFullPath, fileSystem);
        }
    }
}
