using Moq;
using NUnit.Framework;
using PerfectCode.FileSystemIO;
using PerfectCode.TvDbRestApi;
using PerfectCode.TVShows;

namespace PerfectCode.TVShowManager.TV_Show_Manager.TVShowsOnDisk.Tests
{
    [TestFixture]
    public class EpisodeTests
    {
        private Mock<ISeason> _seasonMock;
        private Mock<IFileSystem> _fileSystemMock;
        private Mock<ITVShowRestClient> _tvShowRestClientMock;
        private Mock<IRestApiEpisode> _restApiEpisodeMock;

        [SetUp]
        public void SetUp()
        {
            _seasonMock = new Mock<ISeason>();
            _fileSystemMock = new Mock<IFileSystem>();
            _tvShowRestClientMock = new Mock<ITVShowRestClient>();
            _restApiEpisodeMock = new Mock<IRestApiEpisode>();
        }

        [Test]
        public void UpdateFromWeb()
        {
            _restApiEpisodeMock.Setup(restApiEpisode => restApiEpisode.Name).Returns("Moar blood");
            _restApiEpisodeMock.Setup(restApiEpisode => restApiEpisode.AirDate).Returns("2014-01-27");

            _seasonMock.Setup(season => season.TVShow).Returns(new Mock<ITVShow>().Object);

            var episode = new Episode(_seasonMock.Object, 1, string.Empty, _fileSystemMock.Object);
            episode.Update(_restApiEpisodeMock.Object);

            Assert.AreEqual("Moar blood", episode.Name);
            Assert.AreEqual(27, episode.AirDate.Day);
        }
    }
}
