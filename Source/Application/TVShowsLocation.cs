using System.Collections.Generic;
using System.IO;
using System.Linq;
using PerfectCode.FileSystemIO;
using PerfectCode.TVShowManager.Application;

namespace PerfectCode.TVShows
{
    public class TVShowsLocation : ITVShowLocation
    {
        private readonly IFileSystem _fileSystem;

        public TVShowsLocation(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public List<ITVShow> Locate(string path, ITVShowFactory tvShowFactory, IFileSystem fileSystem)
        {
            if (!_fileSystem.DirectoryExists(path))
            {
                throw new DirectoryNotFoundException(path);
            }

            var tvShows = new List<ITVShow>();
            foreach (var tvShowFolder in _fileSystem.EnumerateDirectories(path))
            {
                var tvShow = tvShowFactory.Create(tvShowFolder, fileSystem);
                tvShow.CheckFileSystem(new SeasonsFactory(fileSystem));
                tvShows.Add(tvShow);
            }
            return tvShows;
        }
    }
}
