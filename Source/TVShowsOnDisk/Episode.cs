using System;
using System.Collections.Generic;
using System.Linq;
using PerfectCode.FileSystemIO;
using PerfectCode.TvDbRestApi;

namespace PerfectCode.TVShows
{
    public class Episode : IEpisode
    {
        public event EventHandler PropertyChanged;

        private readonly IFileSystem _fileSystem;

        public string EpisodeFullName { get; set; }
        public ISeason Season { get; }
        public int Number { get; }
        public string Name { get; set; }
        public DateTime AirDate { get; set; }

        public bool CanDownload => !IsPresentOnDisk && AiredAtLeastOneDayAgo() && AirDate != DateTime.MaxValue;
        public bool ComingSoon => AirDate > DateTime.Now;
        public bool IsPresentOnDisk
        {
            get
            {
                if (string.IsNullOrEmpty(EpisodeFullName))
                {
                    return false;
                }
                return _fileSystem.FileExists(EpisodeFullName);
            }
        }

        public bool IsNew
        {
            get
            {
                var fileNameWithoutExtension = _fileSystem.GetFileNameWithoutExtension(EpisodeFullName);
                var subtitleFiles = _fileSystem.EnumerateFiles(Season.Path, false, new List<string> { $"{fileNameWithoutExtension}*srt" });
                return !ComingSoon && !subtitleFiles.Any();
            }
        }

        public Episode(ISeason season, int number, string episodeFullName, IFileSystem fileSystem)
        {
            Season = season;
            EpisodeFullName = episodeFullName;
            _fileSystem = fileSystem;

            Number = number;
            AirDate = DateTime.MaxValue;
        }

        public void Update(IRestApiEpisode episodeInfo)
        {
            Name = episodeInfo.Name;

            if (!string.IsNullOrEmpty(episodeInfo.AirDate))
            {
                var airDate = DateTime.Parse(episodeInfo.AirDate);
                AirDate = airDate;
            }
            FirePropertyChanged();
        }

        public void OpenInExplorer()
        {
            _fileSystem.OpenInExplorer(EpisodeFullName);
        }

        public void MarkAsWatched()
        {
            var subtitleFullName = _fileSystem.ReplaceExtension(EpisodeFullName, "srt");
            _fileSystem.CreateFile(subtitleFullName);
        }

        public void MarkAsUnwatched()
        {
            var fileName = _fileSystem.GetFileNameWithoutExtension(EpisodeFullName);
            var subtitles = _fileSystem.EnumerateFiles(Season.Path, false, new List<string> {$"{fileName}*.*srt"});
            foreach (var subtitle in subtitles)
            {
                _fileSystem.DeleteFile(subtitle);
            }
        }

        private bool AiredAtLeastOneDayAgo()
        {
            return AirDate < DateTime.Now.Subtract(new TimeSpan(1,0,0,0));
        }

        public override string ToString()
        {
            return $"{Number} - {Name}";
        }

        private void FirePropertyChanged()
        {
            PropertyChanged?.Invoke(this, new EventArgs());
            Season.FirePropertyChanged();
        }
    }
}