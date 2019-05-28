using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using PerfectCode.TvDbRestApi;
using PerfectCode.TVShows;

namespace PerfectCode.TVShowManager.UserInterface.ViewModels.CreateTVShowsLibrary
{
    public class FindMatchingTVShowViewModel : WizardPageViewModel
    {
        private readonly ICreateTVShowsLibraryWizardViewModel _parent;
        private readonly ITVShowRestClient _restApi;
        private CandidateTVShowViewModel _selectedCandidate;
        public string TVShowFullName => TVShow.Path;

        public ITVShow TVShow { get; }

        public CandidateTVShowViewModel SelectedCandidate
        {
            get { return _selectedCandidate; }
            set { Set(() => SelectedCandidate, ref _selectedCandidate, value); }
        }

        public ObservableCollection<CandidateTVShowViewModel> Candidates { get; }

        public FindMatchingTVShowViewModel(ICreateTVShowsLibraryWizardViewModel parent, ITVShow tvShow, ITVShowRestClient restApi)
        {
            _parent = parent;
            _restApi = restApi;
            TVShow = tvShow;

            DisplayName = "The following TV Shows are found as candidate";

            Candidates = new ObservableCollection<CandidateTVShowViewModel>();
        }

        public override bool CanNext()
        {
            var canNext = base.CanNext();

            canNext &= Candidates.Any(candidate => candidate.IsSelected);

            return canNext;
        }

        public override async Task OnPageActivated()
        {
            await base.OnPageActivated();

            var onlineCandidates = await _restApi.GetTvShowsByName(TVShow.Name);

            foreach (var restApiTVShowResult in onlineCandidates)
            {
                var posterPath = await _restApi.GetPosterImageUrl(restApiTVShowResult.PosterPath);

                Candidates.Add(new CandidateTVShowViewModel(restApiTVShowResult, posterPath));
            }

            SelectedCandidate = Candidates.FirstOrDefault();
        }

        public override void OnNextExecuted()
        {
            base.OnNextExecuted();

            if (!Skipped)
            {
                _parent.SetMatchedCandidate(TVShow, Candidates.Where(candidate => candidate.IsSelected).Select(candidate => candidate.RestTVShow).First());
            }
        }
    }
}
