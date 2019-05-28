using Moq;
using NUnit.Framework;
using PerfectCode.FileSystemIO;
using PerfectCode.TvDbRestApi;
using PerfectCode.TVShows;

namespace PerfectCode.TVShowManager.TV_Show_Manager.Application.Tests
{
    [TestFixture]
    public class TVShowLibraryTests
    {
        private Mock<ITVShow> _tvShowOnDiskMock;
        private Mock<ITVShowRestClient> _tvShowRestApiClient;
        private Mock<IFileSystem> _fileSystemMock;

        [SetUp]
        public void SetUp()
        {
            _tvShowOnDiskMock = new Mock<ITVShow>();
            _tvShowRestApiClient = new Mock<ITVShowRestClient>();
            _fileSystemMock = new Mock<IFileSystem>();
        }

        [Test]
        public void AddTVShowLibraryByPath()
        {
            var tvShowLibrary = new TVShowLibrary(_fileSystemMock.Object, _tvShowRestApiClient.Object);
            tvShowLibrary.TVShowAdded += (sender, args) => Assert.IsNotNull(args.TVShow);
            tvShowLibrary.Add(_tvShowOnDiskMock.Object);
        }
    }
}
