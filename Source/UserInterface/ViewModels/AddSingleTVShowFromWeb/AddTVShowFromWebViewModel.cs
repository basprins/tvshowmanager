using System;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using PerfectCode.TvDbRestApi;
using PerfectCode.TVShowManager.UserInterface.ViewModels.CreateTVShowsLibrary;

namespace PerfectCode.TVShowManager.UserInterface.ViewModels.AddSingleTVShowFromWeb
{
    public class AddTVShowFromWebViewModel : ViewModelBase
    {
        private readonly ITVShowRestClient _restClient;
        private CandidateTVShowViewModel _selectedCandidate;
        private bool? _dialogResult;
        private TVShowSearchType _searchType;

        private int _currentSearchPage;
        private int _totalSearchPages;

        private int _currentDiscoveryPage;
        private int _totalDiscoveryPages;

        public string TVShowSearchString { get; set; }

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(() => DialogResult, ref _dialogResult, value); }
        }

        public TVShowSearchType SearchType
        {
            get { return _searchType; }
            set { Set(() => SearchType, ref _searchType, value); }
        }

        public CandidateTVShowViewModel SelectedCandidate
        {
            get { return _selectedCandidate; }
            set { Set(() => SelectedCandidate, ref _selectedCandidate, value); }
        }

        public ObservableCollection<CandidateTVShowViewModel> Candidates { get; }

        public IRestApiTVShow NewTVShow => SelectedCandidate?.RestTVShow;

        public RelayCommand PreviousResultsCommand { get; }
        public RelayCommand NextResultsCommand { get; }

        public RelayCommand SearchTVShowCommand { get; }
        public RelayCommand DiscoverTVShowsCommand { get; }
        public RelayCommand FinishCommand { get; }

        public AddTVShowFromWebViewModel(ITVShowRestClient restClient)
        {
            _restClient = restClient;

            Candidates = new ObservableCollection<CandidateTVShowViewModel>();

            PreviousResultsCommand = new RelayCommand(PreviousPage, CanNavigateToPreviousPage);
            NextResultsCommand = new RelayCommand(NextPage, CanNavigateToNextPage);

            SearchTVShowCommand = new RelayCommand(SearchTVShow, CanSearch);
            DiscoverTVShowsCommand = new RelayCommand(Discover);
            FinishCommand = new RelayCommand(Finish, CanFinish);

            SearchType = TVShowSearchType.SearchByName;
        }

        private async void PreviousPage()
        {
            _currentDiscoveryPage--;

            Candidates.Clear();

            var restApiTVShows = await _restClient.DiscoverTVShows(DateTime.MaxValue, DateTime.MaxValue, TVShowDiscoveryType.EpisodeAirDate, _currentDiscoveryPage);
            foreach (var restApiTVShow in restApiTVShows.Results)
            {
                var posterUrl = await _restClient.GetPosterImageUrl(restApiTVShow.PosterPath);
                Candidates.Add(new CandidateTVShowViewModel(restApiTVShow, posterUrl));
            }

            SelectedCandidate = Candidates.FirstOrDefault();
        }

        private bool CanNavigateToPreviousPage()
        {
            switch (SearchType)
            {
                case TVShowSearchType.Discover:
                    return _currentDiscoveryPage > 0;
                case TVShowSearchType.SearchByName:
                    return _currentSearchPage > 0;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async void NextPage()
        {
            _currentDiscoveryPage++;

            Candidates.Clear();

            var restApiTVShows = await _restClient.DiscoverTVShows(DateTime.MaxValue, DateTime.MaxValue, TVShowDiscoveryType.EpisodeAirDate, _currentDiscoveryPage);
            foreach (var restApiTVShow in restApiTVShows.Results)
            {
                var posterUrl = await _restClient.GetPosterImageUrl(restApiTVShow.PosterPath);
                Candidates.Add(new CandidateTVShowViewModel(restApiTVShow, posterUrl));
            }

            SelectedCandidate = Candidates.FirstOrDefault();
        }

        private bool CanNavigateToNextPage()
        {
            switch (SearchType)
            {
                case TVShowSearchType.Discover:
                    return _currentDiscoveryPage < _totalDiscoveryPages;
                case TVShowSearchType.SearchByName:
                    return _currentSearchPage < _totalSearchPages;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async void SearchTVShow()
        {
            Candidates.Clear();

            var restApiTVShows = await _restClient.GetTvShowsByName(TVShowSearchString);

            foreach (var restApiTVShow in restApiTVShows)
            {
                var posterUrl = await _restClient.GetPosterImageUrl(restApiTVShow.PosterPath);
                Candidates.Add(new CandidateTVShowViewModel(restApiTVShow, posterUrl));
            }

            SelectedCandidate = Candidates.FirstOrDefault();
        }

        private bool CanSearch()
        {
            return !string.IsNullOrEmpty(TVShowSearchString);
        }

        private async void Discover()
        {
            Candidates.Clear();

            _currentDiscoveryPage = 0;

            var restApiTVShows = await _restClient.DiscoverTVShows(DateTime.MaxValue, DateTime.MaxValue);
            _totalDiscoveryPages = restApiTVShows.TotalPages;

            foreach (var restApiTVShow in restApiTVShows.Results)
            {
                var posterUrl = await _restClient.GetPosterImageUrl(restApiTVShow.PosterPath);
                Candidates.Add(new CandidateTVShowViewModel(restApiTVShow, posterUrl));
            }

            SelectedCandidate = Candidates.FirstOrDefault();
        }

        private void Finish()
        {
            DialogResult = true;
        }

        private bool CanFinish()
        {
            return SelectedCandidate != null;
        }
    }

    public enum TVShowSearchType
    {
        Discover,
        SearchByName,
    }
}
