using System.Collections.Generic;
using System.Text.RegularExpressions;
using PerfectCode.FileSystemIO;
using PerfectCode.TVShows;

namespace PerfectCode.TVShowManager.Application
{
    public class SeasonsFactory : ISeasonFactory
    {
        private readonly Regex _seasonRegex = new Regex(@"Season\s(?<seasonNumber>\d+)", RegexOptions.Compiled);

        private readonly IFileSystem _fileSystem;

        public SeasonsFactory(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public List<ISeason> Create(ITVShow tvShow)
        {
            var seasons = new List<ISeason>();
            foreach (var directory in _fileSystem.EnumerateDirectories(tvShow.Path))
            {
                var match = _seasonRegex.Match(directory);
                if (match.Success)
                {
                    var seasonNumber = int.Parse(match.Groups["seasonNumber"].Value);
                    seasons.Add(new Season(tvShow, directory, seasonNumber, new EpisodesFactory(_fileSystem), _fileSystem));
                }
            }

            return seasons;
        }
    }
}
