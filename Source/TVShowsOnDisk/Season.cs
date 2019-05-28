using System;
using System.Collections.Generic;
using System.Linq;
using PerfectCode.FileSystemIO;
using PerfectCode.TvDbRestApi;
using PerfectCode.TVShowManager.Application;

namespace PerfectCode.TVShows
{
    public class Season : ISeason
    {
        public event EventHandler PropertyChanged;

        private readonly IFileSystem _fileSystem;

        public ITVShow TVShow { get; set; }
        public bool Updating { get; set; }

        public string Path { get; set; }

        public int Number { get; set; }

        public List<IEpisode> Episodes { get; set; }

        public Season(ITVShow tvShow, string seasonFullPath, int number, IEpisodeFactory episodeFactory, IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            TVShow = tvShow;
            Number = number;
            Path = seasonFullPath;

            Episodes = episodeFactory.Create(this, seasonFullPath, fileSystem);
        }

        public async void Update(IRestApiSeason seasonInfo, ITVShowRestClient restApiClient)
        {
            if (TVShow.WebID.HasValue)
            {
                // Check if there are online episodes which are missing on disk
                var notifyChanges = false;

                var season = await restApiClient.GetTvShowSeasonEpisodes(TVShow.WebID.Value, Number);
                if (season != null)
                {
                    foreach (var apiEpisode in season.Episodes)
                    {
                        var episodeInLibrary = Episodes.FirstOrDefault(episode => episode.Number == apiEpisode.Number);
                        if (episodeInLibrary == null)
                        {
                            var newEpisode = new Episode(this, apiEpisode.Number, string.Empty, _fileSystem)
                            {
                                Name = apiEpisode.Name
                            };
                            Episodes.Add(newEpisode);
                            notifyChanges = true;
                        }
                        else
                        {
                            episodeInLibrary.Update(apiEpisode);
                        }
                    }
                }
                if (notifyChanges)
                {
                    FirePropertyChanged();
                }
            }
        }

        public override string ToString()
        {
            return $"'Season {Number}' - {Episodes.Count} episodes";
        }

        public void FirePropertyChanged()
        {
            PropertyChanged?.Invoke(this, new EventArgs());
            TVShow.FirePropertyChanged();
        }
    }
}