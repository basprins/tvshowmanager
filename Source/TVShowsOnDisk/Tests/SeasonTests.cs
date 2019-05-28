using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using PerfectCode.FileSystemIO;
using PerfectCode.TvDbRestApi;
using PerfectCode.TVShowManager.Application;
using PerfectCode.TVShows;

namespace PerfectCode.TVShowManager.TV_Show_Manager.TVShowsOnDisk.Tests
{
    [TestFixture]
    public class SeasonTests
    {
        private Mock<ITVShow> _tvShowMock;
        private Mock<IEpisodeFactory> _episodeFactoryMock;
        private Mock<IFileSystem> _fileSystemMock;
        private Mock<ITVShowRestClient> _tvShowRestApi;
        private Mock<IRestApiSeason> _restApiSeason;

        [SetUp]
        public void SetUp()
        {
            _tvShowMock = new Mock<ITVShow>();
            _episodeFactoryMock = new Mock<IEpisodeFactory>();
            _fileSystemMock = new Mock<IFileSystem>();
            _tvShowRestApi = new Mock<ITVShowRestClient>();
            _restApiSeason = new Mock<IRestApiSeason>();
        }

        [Test]
        public void CheckForUpdates()
        {
            var episodeMock = new Mock<IEpisode>();
            var restApiEpisodeMock = new Mock<IRestApiEpisode>();

            _episodeFactoryMock
                .Setup(episodesFactory => episodesFactory.Create(It.IsAny<ISeason>(), string.Empty, _fileSystemMock.Object))
                .Returns(new List<IEpisode> {episodeMock.Object});

            _restApiSeason
                .Setup(apiSeason => apiSeason.Episodes)
                .Returns(new List<IRestApiEpisode> {restApiEpisodeMock.Object});

            _restApiSeason
                .SetupSequence(apiSeason => apiSeason.Number)
                .Returns(0)
                .Returns(1)
                .Returns(1);

            var season = new Season(_tvShowMock.Object, string.Empty, 1, _episodeFactoryMock.Object, _fileSystemMock.Object);
            season.Update(_restApiSeason.Object, _tvShowRestApi.Object);

            episodeMock.Verify(episode => episode.Update(restApiEpisodeMock.Object));
        }
    }
}
