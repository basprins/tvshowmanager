using System.Collections.Generic;
using System.Text.RegularExpressions;
using PerfectCode.FileSystemIO;
using PerfectCode.TVShows;

namespace PerfectCode.TVShowManager.Application
{
    public class EpisodesFactory : IEpisodeFactory
    {
        private readonly Regex _episodeRegex = new Regex(@"S\d{2}E(?<episode>\d{2})", RegexOptions.Compiled);

        private readonly IFileSystem _fileSystem;

        public EpisodesFactory(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public List<IEpisode> Create(ISeason season, string seasonFullPath, IFileSystem fileSystem)
        {
            var episodes = new List<IEpisode>();

            if (_fileSystem.DirectoryExists(season.Path))
            {
                foreach (var file in _fileSystem.EnumerateFiles(season.Path, false, new List<string> { ".mkv", ".avi", ".mp4" }))
                {
                    var match = _episodeRegex.Match(file);
                    if (match.Success)
                    {
                        var episodeNumber = int.Parse(match.Groups["episode"].Value);
                        episodes.Add(new Episode(season, episodeNumber, file, _fileSystem));
                    }
                }
            }

            return episodes;
        }
    }
}
