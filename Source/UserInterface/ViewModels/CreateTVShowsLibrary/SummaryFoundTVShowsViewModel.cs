using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerfectCode.TVShowManager.UserInterface.ViewModels.CreateTVShowsLibrary
{
    public class SummaryFoundTVShowsViewModel : WizardPageViewModel
    {
        private readonly ICreateTVShowsLibraryWizardViewModel _parent;

        public List<MatchedTVShow> MatchedCandidateOverview { get; }

        public SummaryFoundTVShowsViewModel(ICreateTVShowsLibraryWizardViewModel parent)
        {
            _parent = parent;

            MatchedCandidateOverview = new List<MatchedTVShow>();

            DisplayName = "Summary of TV shows";
        }

        public override async Task OnPageActivated()
        {
            await base.OnPageActivated();

            MatchedCandidateOverview.Clear();
            
            foreach (var matchedTVShow in _parent.MatchedTVShows)
            {
                MatchedCandidateOverview.Add(new MatchedTVShow(matchedTVShow.Key, matchedTVShow.Value));
            }
        }
    }
}
