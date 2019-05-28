using Moq;
using NUnit.Framework;
using PerfectCode.TvDbRestApi;
using PerfectCode.TVShowManager.UserInterface.ViewModels.CreateTVShowsLibrary;
using PerfectCode.TVShows;

namespace PerfectCode.TVShowManager.TV_Show_Manager.UserInterface.ViewModels.CreateTVShowsLibrary.Tests
{
    [TestFixture]
    public class MatchedTVShowTests
    {
        [Test]
        public void CreateMatchedTVShow()
        {
            var tvShowMock = new Mock<ITVShow>();
            var restApiTVShow = new Mock<IRestApiTVShow>();
            var match = new MatchedTVShow(tvShowMock.Object, restApiTVShow.Object);

            Assert.AreEqual(tvShowMock.Object, match.TVShow);
            Assert.AreEqual(restApiTVShow.Object, match.MatchedCandidate);
        }
    }
}
