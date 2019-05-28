using System;
using System.Linq;
using System.Text.RegularExpressions;
using PerfectCode.FileSystemIO;
using PerfectCode.TVShows;

namespace PerfectCode.TVShowManager.UserInterface.ViewModels.ImportEpisodes
{
    public class EpisodeDestination
    {
        private readonly Regex _pureAsciiRegex = new Regex(@"[^\u0000-\u007F]", RegexOptions.Compiled);
        private readonly DownloadedEpisodeViewModel _downloadedEpisode;
        private readonly ITVShow _tvShow;
        public ISeason Season { get; }

        public string OriginalFileName => _downloadedEpisode.FileName;
        public bool IsSkipped => _downloadedEpisode.IsSkipped;

        public string TVShowName => _tvShow.Name;
        public int SeasonNumber => Season.Number;
        public int EpisodeNumber
        {
            get
            {
                if (_downloadedEpisode.EpisodeNumber == null)
                {
                    throw new InvalidOperationException();
                }
                return _downloadedEpisode.EpisodeNumber.Value;
            }
        }

        public string TargetFileName { get; }

        public EpisodeDestination(DownloadedEpisodeViewModel downloadedEpisode, ITVShow tvShow, ISeason season, IFileSystem fileSystem)
        {
            _downloadedEpisode = downloadedEpisode;
            _tvShow = tvShow;
            Season = season;

            var targetEpisode = Season.Episodes.First(episode => episode.Number == EpisodeNumber);
            var extension = fileSystem.GetExtension(OriginalFileName);

            var targetFileName = $"{TVShowName} - S{SeasonNumber:00}E{EpisodeNumber:00} - {targetEpisode.Name}{extension}";

            // Make sure all non ASCII characters are removed
            targetFileName = _pureAsciiRegex.Replace(targetFileName, string.Empty);
            targetFileName = fileSystem.RemoveIllegalCharactersFromFileName(targetFileName);

            TargetFileName = fileSystem.CombinePaths(_tvShow.Path, Season.Path, targetFileName);
        }
    }
}