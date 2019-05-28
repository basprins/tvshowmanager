using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using PerfectCode.TVShows;

namespace PerfectCode.TVShowManager.UserInterface.ViewModels
{
    public class TVShowViewModel : ViewModelBase, ITVShowViewModel
    {
        private readonly ITVShowLibrary _tvShowLibrary;
        public ITVShow TVShow { get; }
        private string _name;
        private SeasonViewModel _selectedSeason;
        private bool _downloadsAvailable;
        private bool _hasNewAvailable;
        private bool _comingSoon;
        private int _numberOfSeasons;
        private string _posterImageUrl;
        private string _backdropImageUrl;

        public string Name
        {
            get { return _name; }
            set { Set(() => Name, ref _name, value); }
        }

        public SeasonViewModel SelectedSeason
        {
            get => _selectedSeason;
            set
            {
                Set(() => SelectedSeason, ref _selectedSeason, value);
                if (value != null)
                {
                    _tvShowLibrary.UpdateSeason(TVShow, value.Season);
                }
            }
        }

        public bool DownloadsAvailable
        {
            get { return _downloadsAvailable; }
            set { Set(() => DownloadsAvailable, ref _downloadsAvailable, value); }
        }

        public bool HasNewEpisodes
        {
            get { return _hasNewAvailable;  }
            set { Set(() => HasNewEpisodes, ref _hasNewAvailable, value); }
        }

        public bool ComingSoon
        {
            get { return _comingSoon; }
            set { Set(() => ComingSoon, ref _comingSoon, value); }
        }

        public int NumberOfSeasons
        {
            get { return _numberOfSeasons; }
            set { Set(() => NumberOfSeasons, ref _numberOfSeasons, value); }
        }

        public BitmapImage PosterImage
        {
            get
            {
                if (!string.IsNullOrEmpty(_posterImageUrl))
                {
                    var bitmapImage = new BitmapImage(new Uri(_posterImageUrl));
                    return bitmapImage;
                }
                return null;
            }
        }

        public BitmapImage BackdropImage
        {
            get
            {
                if (!string.IsNullOrEmpty(_backdropImageUrl))
                {
                    var bitmapImage = new BitmapImage(new Uri(_backdropImageUrl));
                    return bitmapImage;
                }
                return null;
            }
        }

        public int? Id => TVShow.WebID;
        public bool InProduction => TVShow.InProduction;
        public string Homepage => TVShow.Homepage;
        public string Status => TVShow.Status;

        public string Path { get; }

        public ObservableCollection<SeasonViewModel> Seasons { get; }

        public RelayCommand RemoveTVShowCommand { get; }
        public RelayCommand OpenHomepageCommand { get; }

        public TVShowViewModel(ITVShow tvShow, ITVShowLibrary tvShowLibrary)
        {
            _tvShowLibrary = tvShowLibrary;
            TVShow = tvShow;

            Path = Path;
            Seasons = new ObservableCollection<SeasonViewModel>();

            Initialize();

            var lastSeason = Seasons.LastOrDefault();
            SelectedSeason = lastSeason;

            RemoveTVShowCommand = new RelayCommand(Remove);
            OpenHomepageCommand = new RelayCommand(OpenHomePage);

            tvShow.PropertyChanged += TVShowOnPropertyChanged;
        }

        private void OpenHomePage()
        {
            Process.Start(Homepage);
        }

        public override string ToString()
        {
            return $"{TVShow.Name} ({TVShow.Seasons.Count} seasons)";
        }

        private void Remove()
        {
            _tvShowLibrary.RemoveTVShow(TVShow);
        }

        private void Initialize()
        {
            Name = TVShow.Name;
            _posterImageUrl = TVShow.PosterImageUrl;
            _backdropImageUrl = TVShow.BackdropUrl;

            var seasonsAdded = false;
            foreach (var season in TVShow.Seasons)
            {
                if (Seasons.All(seasonViewModel => seasonViewModel.Season.Number != season.Number))
                {
                    Seasons.Add(new SeasonViewModel(season));
                    seasonsAdded = true;
                }
            }

            SeasonViewModel lastSeason = Seasons.LastOrDefault();

            if (seasonsAdded)
            {
                SortSeasons();
                lastSeason = Seasons.Last();
                SelectedSeason = lastSeason;
            }

            DownloadsAvailable = TVShow.DownloadsAvailable;
            ComingSoon = TVShow.ComingSoon;
            HasNewEpisodes = TVShow.HasNewEpisodes;

            NumberOfSeasons = TVShow.Seasons.Count;
        }

        private void SortSeasons()
        {
            var sortedSeasons = Seasons.ToList();
            sortedSeasons = sortedSeasons.OrderBy(season => season.Season.Number).ToList();

            Seasons.Clear();
            
            foreach (var seasonViewModel in sortedSeasons)
            {
                Seasons.Add(seasonViewModel);
            }
        }

        private void TVShowOnPropertyChanged(object sender, EventArgs eventArgs)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(Initialize);
        }
    }
}
