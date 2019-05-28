using System.Collections.Generic;
using PerfectCode.TvDbRestApi;
using PerfectCode.TVShows;

namespace PerfectCode.TVShowManager.UserInterface.ViewModels.CreateTVShowsLibrary
{
    public interface ICreateTVShowsLibraryWizardViewModel
    {
        WizardPageViewModel CurrentPage { get; set; }
        Dictionary<ITVShow, IRestApiTVShow> MatchedTVShows { get; }
        string SelectedTVShowsFolderPath { get; set; }
        void SetTVShowPages(List<ITVShow> tvShows);
        void SetMatchedCandidate(ITVShow tvShow, IRestApiTVShow matchedCandidate);
    }
}