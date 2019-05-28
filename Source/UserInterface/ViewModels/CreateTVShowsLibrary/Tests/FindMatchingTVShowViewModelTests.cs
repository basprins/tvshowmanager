using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using PerfectCode.TvDbRestApi;
using PerfectCode.TVShowManager.UserInterface.ViewModels.CreateTVShowsLibrary;
using PerfectCode.TVShows;

namespace PerfectCode.TVShowManager.TV_Show_Manager.UserInterface.ViewModels.CreateTVShowsLibrary.Tests
{
    [TestFixture]
    public class FindMatchingTVShowViewModelTests
    {
        private Mock<ICreateTVShowsLibraryWizardViewModel> _libraryWizardMock;
        private Mock<ITVShow> _tvShowMock;
        private Mock<ITVShowRestClient> _tvShowRestClientMock;
        private Mock<IRestApiTVShow> _restApiTVShowMock;

        [SetUp]
        public void SetUp()
        {
            _libraryWizardMock = new Mock<ICreateTVShowsLibraryWizardViewModel>();
            _tvShowMock = new Mock<ITVShow>();
            _restApiTVShowMock = new Mock<IRestApiTVShow>();
            _tvShowRestClientMock = new Mock<ITVShowRestClient>();
        }

        [Test]
        public void CanNext()
        {
            var findMatchingTVShowViewModel = new FindMatchingTVShowViewModel(_libraryWizardMock.Object, _tvShowMock.Object, _tvShowRestClientMock.Object);
            var candidateTVShowViewModel = new CandidateTVShowViewModel(_restApiTVShowMock.Object, string.Empty) {IsSelected = true};
            findMatchingTVShowViewModel.Candidates.Add(candidateTVShowViewModel);
            var canNext = findMatchingTVShowViewModel.CanNext();
            Assert.IsTrue(canNext);
        }

        [Test]
        public async void OnPageActivated()
        {
            _tvShowRestClientMock
                .Setup(tvShowRestClient => tvShowRestClient.GetTvShowsByName(It.IsAny<string>()))
                .Returns(Task.FromResult(new List<IRestApiTVShow> { _restApiTVShowMock.Object }));

            var findMatchingTVShowViewModel = new FindMatchingTVShowViewModel(_libraryWizardMock.Object, _tvShowMock.Object, _tvShowRestClientMock.Object);
            await findMatchingTVShowViewModel.OnPageActivated();

            Assert.AreEqual(_restApiTVShowMock.Object, findMatchingTVShowViewModel.Candidates.First().RestTVShow);
        }

        [Test]
        public void OnNextExecuted()
        {
            var candidateTVShowViewModel = new CandidateTVShowViewModel(_restApiTVShowMock.Object, string.Empty) { IsSelected = true };

            var findMatchingTVShowViewModel = new FindMatchingTVShowViewModel(_libraryWizardMock.Object, _tvShowMock.Object, _tvShowRestClientMock.Object);
            findMatchingTVShowViewModel.Candidates.Add(candidateTVShowViewModel);

            findMatchingTVShowViewModel.OnNextExecuted();

            _libraryWizardMock.Verify(libraryWizard => libraryWizard.SetMatchedCandidate(_tvShowMock.Object, _restApiTVShowMock.Object));
        }
    }
}
