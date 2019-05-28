using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using PerfectCode.FileSystemIO;
using PerfectCode.TvDbRestApi;
using PerfectCode.TVShowManager.Application;
using PerfectCode.TVShows;

namespace PerfectCode.TVShowManager.TV_Show_Manager.TVShowsOnDisk.Tests
{
    [TestFixture]
    public class TVShowTests
    {
        private Mock<ISeasonFactory> _seasonFactoryMock;
        private Mock<IFileSystem> _fileSystemMock;
        private Mock<ITVShowRestClient> _tvShowRestApiMock;

        [SetUp]
        public void SetUp()
        {
            _seasonFactoryMock = new Mock<ISeasonFactory>();
            _fileSystemMock = new Mock<IFileSystem>();
            _tvShowRestApiMock = new Mock<ITVShowRestClient>();
        }

        [Test]
        public void CheckForUpdates()
        {
            var seasonMock = new Mock<ISeason>();
            var restApiSeasonMock = new Mock<IRestApiSeason>();
            var detailedTVInfoMock = new Mock<IRestApiTVShowDetailedInfoResult>();

            detailedTVInfoMock
                .Setup(tvShowInfo => tvShowInfo.Seasons)
                .Returns(new List<IRestApiSeason> {restApiSeasonMock.Object});

            _seasonFactoryMock
                .Setup(seasonsFactory => seasonsFactory.Create(It.IsAny<ITVShow>()))
                .Returns(new List<ISeason> {seasonMock.Object});

            _tvShowRestApiMock
                .Setup(restApiClient => restApiClient.GetDetailedTvShowInfo(It.IsAny<int>()))
                .Returns(Task.FromResult(detailedTVInfoMock.Object));

            var tvShow = new TVShow(string.Empty, _fileSystemMock.Object)
            {
                WebID = 123
            };
            tvShow.CheckFileSystem(_seasonFactoryMock.Object);
            tvShow.CheckForUpdates(_tvShowRestApiMock.Object);

            seasonMock.Verify(season => season.Update(restApiSeasonMock.Object, _tvShowRestApiMock.Object));
        }

        [Test]
        public void CheckForUpdatesWhileWebIDIsNull()
        {
            var tvShow = new TVShow(string.Empty, _fileSystemMock.Object);
            tvShow.CheckFileSystem(_seasonFactoryMock.Object);

            Assert.That(() => tvShow.CheckForUpdates(_tvShowRestApiMock.Object), Throws.TypeOf<InvalidOperationException>());
        }
    }
}
