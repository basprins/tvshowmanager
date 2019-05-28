using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using PerfectCode.TVShows;

namespace PerfectCode.TVShowManager.UserInterface.ViewModels
{
    public class SeasonViewModel : ViewModelBase
    {
        public ISeason Season { get; }

        public string Name => $"Season {Season.Number}";

        public ObservableCollection<EpisodeViewModel> Episodes { get; }

        public RelayCommand OpenTorrentSiteCommand { get; }
        public RelayCommand DownloadSelectedCommand { get; }
        public RelayCommand DownloadAvailableCommand { get; }

        public SeasonViewModel(ISeason season)
        {
            Season = season;

            Episodes = new ObservableCollection<EpisodeViewModel>();

            AddEpisodes();

            OpenTorrentSiteCommand = new RelayCommand(OpenTorrentSite);
            DownloadSelectedCommand = new RelayCommand(DownloadSelectedEpisodes, CanDownloadSelectedEpisodes);
            DownloadAvailableCommand = new RelayCommand(DownloadAvailableEpisodes, CanDownloadAvailableEpisodes);
            Season.PropertyChanged += SeasonOnPropertyChanged;
        }

        public override bool Equals(object obj)
        {
            if (obj is SeasonViewModel other)
            {
                return Season.Equals(other.Season);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Season.Number.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Name} ({Season.Episodes.Count} episodes";
        }

        private void OpenTorrentSite()
        {
            var tvShowName = Uri.EscapeDataString(Season.TVShow.Name);

            Process.Start($"http://kickass.to/usearch/{tvShowName}/");
        }

        private void DownloadSelectedEpisodes()
        {
            foreach (var episode in Episodes.Where(episode => episode.IsSelected))
            {
                DownloadEpisode(episode.Episode);
            }
        }

        private bool CanDownloadSelectedEpisodes()
        {
            return Episodes.Any(episode => episode.IsSelected);
        }

        private void DownloadAvailableEpisodes()
        {
            var availableEpisodes = Season.Episodes.Where(episode => episode.CanDownload);
            availableEpisodes = availableEpisodes.OrderBy(episode => episode.Number);
            foreach (var availableEpisode in availableEpisodes)
            {
                DownloadEpisode(availableEpisode);
            }
        }

        private void DownloadEpisode(IEpisode availableEpisode)
        {
            var tvShowEpisodeIdentifier = $"{Season.TVShow.Name} S{Season.Number:00}E{availableEpisode.Number:00}";
            tvShowEpisodeIdentifier = Uri.EscapeDataString(tvShowEpisodeIdentifier);
            Process.Start($"https://kickass.unblocked.la/usearch/{tvShowEpisodeIdentifier}/");
            Thread.Sleep(100);
        }

        private bool CanDownloadAvailableEpisodes()
        {
            return Season.Episodes.Any(episode => !episode.IsPresentOnDisk && episode.AirDate < DateTime.Now);
        }

        private void AddEpisodes()
        {
            var episodesAdded = false;
            foreach (var episode in Season.Episodes)
            {
                if (Episodes.All(episodeViewModel => episodeViewModel.Episode.Number != episode.Number))
                {
                    Episodes.Add(new EpisodeViewModel(episode));
                    episodesAdded = true;
                }
            }

            if (episodesAdded)
            {
                var sortedEpisodes = Episodes.ToList();
                sortedEpisodes = sortedEpisodes.OrderBy(episode => episode.Number).ToList();

                Episodes.Clear();

                foreach (var episodeViewModel in sortedEpisodes)
                {
                    Episodes.Add(episodeViewModel);
                }
            }
        }

        private void SeasonOnPropertyChanged(object sender, EventArgs eventArgs)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(AddEpisodes);
        }
    }
}