using PerfectCode.FileSystemIO;
using PerfectCode.TVShows;

namespace PerfectCode.TVShowManager.Application
{
    public interface ITVShowFactory
    {
        ITVShow Create(string tvShowFullPath, IFileSystem fileSystem);
    }
}