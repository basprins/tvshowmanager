using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using PerfectCode.FileSystemIO;
using PerfectCode.TvDbRestApi;
using PerfectCode.TVShows;

namespace PerfectCode.TVShowManager.UserInterface.ViewModels.CreateTVShowsLibrary
{
    public class CreateTVShowsLibraryWizardViewModel : ViewModelBase, ICreateTVShowsLibraryWizardViewModel
    {
        private readonly ITVShowRestClient _restApi;
        private WizardPageViewModel _currentPage;
        private bool? _dialogResult;
        private bool _wizardReady;
        private bool _wizardInProgress;

        public List<WizardPageViewModel> Pages { get; }

        public WizardPageViewModel CurrentPage
        {
            get => _currentPage;
            set { Set(() => CurrentPage, ref _currentPage, value); }
        }

        public bool WizardReady
        {
            get => _wizardReady;
            set { Set(() => WizardReady, ref _wizardReady, value); }
        }

        public bool WizardInProgress
        {
            get => _wizardInProgress;
            set { Set(() => WizardInProgress, ref _wizardInProgress, value); }
        }

        public bool? DialogResult
        {
            get => _dialogResult;
            set { Set(() => DialogResult, ref _dialogResult, value); }
        }

        public RelayCommand PreviousCommand { get; set; }
        public RelayCommand NextCommand { get; set; }
        public RelayCommand FinishCommand { get; set; }
        public RelayCommand SkipCommand { get; set; }

        public Dictionary<ITVShow, IRestApiTVShow> MatchedTVShows { get; }
        public string SelectedTVShowsFolderPath { get; set; }

        public CreateTVShowsLibraryWizardViewModel(IFileSystem fileSystem, IDialogService dialogService, ITVShowRestClient restApi, ITVShowLibrary tvShowLibrary)
        {
            _restApi = restApi;

            MatchedTVShows = new Dictionary<ITVShow, IRestApiTVShow>();

            var chooseTVShowsFolderViewModel = new ChooseTVShowsFolderViewModel(this, new TVShowsLocation(fileSystem), dialogService, fileSystem, tvShowLibrary);
            Pages = new List<WizardPageViewModel>
            {
                chooseTVShowsFolderViewModel
            };

            CurrentPage = chooseTVShowsFolderViewModel;

            PreviousCommand = new RelayCommand(Previous, CanPrevious);
            NextCommand = new RelayCommand(Next, CanNext);
            FinishCommand = new RelayCommand(Finish, CanFinish);
            SkipCommand = new RelayCommand(Skip);
            WizardInProgress = true;
        }

        public void SetTVShowPages(List<ITVShow> tvShows)
        {
            // Make sure the previous added pages are removed before adding new ones
            for (var i = Pages.Count - 1; i > 0; i--)
            {
                Pages.RemoveAt(i);
            }

            // Add the TV Show pages
            foreach (var tvShow in tvShows)
            {
                Pages.Add(new FindMatchingTVShowViewModel(this, tvShow, _restApi));
            }

            // Added summary page
            Pages.Add(new SummaryFoundTVShowsViewModel(this));
        }

        public void SetMatchedCandidate(ITVShow tvShow, IRestApiTVShow matchedCandidate)
        {
            MatchedTVShows[tvShow] = matchedCandidate;
        }

        private void Previous()
        {
            var reversedPages = Pages.ToList();
            reversedPages.Reverse();
            CurrentPage = reversedPages.SkipWhile(page => page != CurrentPage).Skip(1).First();
            WizardInProgress = true;
            WizardReady = false;
        }

        private bool CanPrevious()
        {
            return CurrentPage != Pages.First();
        }

        private async void Next()
        {
            CurrentPage.OnNextExecuted();
            CurrentPage = Pages.SkipWhile(page => page != CurrentPage).Skip(1).First();
            await CurrentPage.OnPageActivated();

            var lastPageActive = CurrentPage == Pages.Last();
            WizardReady = lastPageActive;
            WizardInProgress = !lastPageActive;

            NextCommand.RaiseCanExecuteChanged();
        }

        private bool CanNext()
        {
            return CurrentPage.CanNext() && CurrentPage != Pages.Last();
        }

        private void Finish()
        {
            foreach (var tvShow in MatchedTVShows.Keys)
            {
                var matchedCandidate = MatchedTVShows[tvShow];
                tvShow.WebID = matchedCandidate.Id;
            }
            DialogResult = true;
        }

        private bool CanFinish()
        {
            return CurrentPage == Pages.Last();
        }

        private async void Skip()
        {
            CurrentPage.Skipped = true;

            CurrentPage.OnNextExecuted();
            CurrentPage = Pages.SkipWhile(page => page != CurrentPage).Skip(1).First();
            await CurrentPage.OnPageActivated();

            var lastPageActive = CurrentPage == Pages.Last();
            WizardReady = lastPageActive;
            WizardInProgress = !lastPageActive;

            NextCommand.RaiseCanExecuteChanged();
        }
    }
}
