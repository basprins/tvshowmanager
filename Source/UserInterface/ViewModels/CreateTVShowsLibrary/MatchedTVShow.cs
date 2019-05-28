using PerfectCode.TvDbRestApi;
using PerfectCode.TVShows;

namespace PerfectCode.TVShowManager.UserInterface.ViewModels.CreateTVShowsLibrary
{
    public class MatchedTVShow
    {
        public ITVShow TVShow { get; }
        public IRestApiTVShow MatchedCandidate { get; }

        public MatchedTVShow(ITVShow tvShow, IRestApiTVShow matchedCandidate)
        {
            TVShow = tvShow;
            MatchedCandidate = matchedCandidate;
        }
    }
}