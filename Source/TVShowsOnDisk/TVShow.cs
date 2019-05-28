using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using PerfectCode.FileSystemIO;
using PerfectCode.TvDbRestApi;
using PerfectCode.TVShowManager.Application;

namespace PerfectCode.TVShows
{
    [DataContract]
    public class TVShow : ITVShow
    {
        public event EventHandler PropertyChanged;

        [DataMember]
        private readonly IFileSystem _fileSystem;

        private string _name;
        private bool _updating;

        public string PosterImageUrl { get; private set; }
        public string BackdropUrl { get; private set; }

        [DataMember]
        public string Path { get; set; }

        [DataMember]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                FirePropertyChanged();
            }
        }

        [DataMember]
        public int? WebID { get; set; }

        [IgnoreDataMember]
        public List<ISeason> Seasons { get; set; }

        public bool DownloadsAvailable
        {
            get
            {
                // Find the first episode (walking back from the last season to the first) which has an episode which is NOT coming soon, and is available for download
                return (from season in Seasons.OrderByDescending(season => season.Number)
                    from episode in season.Episodes.OrderByDescending(episode => episode.Number)
                    where !episode.ComingSoon
                    select episode.CanDownload).FirstOrDefault();
            }
        }

        public bool HasNewEpisodes
        {
            get
            {
                // Find the first episode (walking back from the last season to the first) which has an episode which is NOT coming soon, and is available for download
                return (from season in Seasons.OrderByDescending(season => season.Number)
                        from episode in season.Episodes.OrderByDescending(episode => episode.Number)
                        where !episode.ComingSoon && !episode.CanDownload
                        select episode.IsNew).FirstOrDefault();
            }
        }

        public bool ComingSoon
        {
            get
            {
                var lastSeason = Seasons.FirstOrDefault(season => season.Number == Seasons.Max(s => s.Number));

                // Check if the last episode of last season is available for download
                var lastEpisode = lastSeason?.Episodes.LastOrDefault();
                if (lastEpisode != null)
                {
                    return lastEpisode.ComingSoon;
                }

                return false;
            }
        }

        public bool Updating
        {
            get => _updating;
            set
            {
                _updating = value;
                PropertyChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool InProduction { get; set; }
        public string Homepage { get; set; }
        public string Status { get; set; }

        public TVShow(string tvShowPath, IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;

            Seasons = new List<ISeason>();

            Path = tvShowPath;
            Name = fileSystem.GetLastSubDirectoryName(tvShowPath);
        }

        [OnDeserialized]
        private void OnDeserializing(StreamingContext c)
        {
            Seasons = new List<ISeason>();
        }

        public void CheckFileSystem(ISeasonFactory seasonsFactory)
        {
            Seasons = seasonsFactory.Create(this);
            FirePropertyChanged();
        }

        public async void CheckForUpdates(ITVShowRestClient restApiClient)
        {
            if (WebID == null)
            {
                throw new InvalidOperationException("WebID can't be null when checking for updates");
            }

            Updating = true;

            var detailedTVShowInfo = await restApiClient.GetDetailedTvShowInfo(WebID.Value);

            PosterImageUrl = await restApiClient.GetPosterImageUrl(detailedTVShowInfo.PosterPath);
            BackdropUrl = await restApiClient.GetBackdropImageUrl(detailedTVShowInfo.BackdropPath);

            InProduction = detailedTVShowInfo.InProduction;
            Homepage = detailedTVShowInfo.Homepage;
            Status = detailedTVShowInfo.Status;
            
            // Check for latest seasons which are not present in library yet
            foreach (var apiSeason in detailedTVShowInfo.Seasons)
            {
                var seasonOnDisk = Seasons.FirstOrDefault(season => season.Number == apiSeason.Number);

                if (seasonOnDisk == null)
                {
                    var season = new Season(
                        this,
                        _fileSystem.CombinePaths(Path, $"Season {apiSeason.Number}"),
                        apiSeason.Number,
                        new EpisodesFactory(_fileSystem),
                        _fileSystem);

                    Seasons.Add(season);
                }
            }

            Updating = false;

            FirePropertyChanged();
        }

        public async void UpdateSeason(ITVShowRestClient restApiClient, ISeason season)
        {
            if (WebID == null)
            {
                throw new InvalidOperationException("WebID can't be null when checking for updates");
            }

            if (!season.Updating)
            {
                var detailedTVShowInfo = await restApiClient.GetDetailedTvShowInfo(WebID.Value);
                var seasonInfo = detailedTVShowInfo.Seasons.FirstOrDefault(s => s.Number == season.Number);
                
                season.Update(seasonInfo, restApiClient);
            }
        }

        public override string ToString()
        {
            return $"{Name} - {Seasons.Count} seasons";
        }

        public override bool Equals(object obj)
        {
            if (obj is TVShow other)
            {
                return Name.Equals(other.Name);
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = 0;

            hashCode ^= Name.GetHashCode();

            return hashCode;
        }

        public void FirePropertyChanged()
        {
            PropertyChanged?.Invoke(this, new EventArgs());
        }
    }
}