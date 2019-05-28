using System.IO;
using Moq;
using NUnit.Framework;
using PerfectCode.FileSystemIO;
using PerfectCode.TVShowManager.Application;
using PerfectCode.TVShows;

namespace PerfectCode.TVShowManager.TVShows.Tests
{
    [TestFixture]
    public class TVShowsLocationTests
    {
        private Mock<IFileSystem> _fileSystemMock;
        private Mock<ITVShowFactory> _tvShowFactoryMock;
        private Mock<ISeasonFactory> _seasonFactoryMock;

        [SetUp]
        public void SetUp()
        {
            _fileSystemMock = new Mock<IFileSystem>();
            _tvShowFactoryMock = new Mock<ITVShowFactory>();
            _seasonFactoryMock = new Mock<ISeasonFactory>();
        }

        [Test]
        public void FindTVShowOnDisk()
        {
            const string path = @"C:\TV shows\";
            var tvShows = new[]
            {
                @"C:\TV shows\24",
                @"C:\TV shows\Dexter",
                @"C:\TV shows\Greys Anatomy"
            };

            _tvShowFactoryMock
                .Setup(tvShowFactory => tvShowFactory.Create(
                    It.IsAny<string>(),
                    _fileSystemMock.Object))
                .Returns(new Mock<ITVShow>().Object);

            _fileSystemMock.Setup(fileSystem => fileSystem.DirectoryExists(path)).Returns(true);
            _fileSystemMock.Setup(fileSystem => fileSystem.EnumerateDirectories(path)).Returns(tvShows);

            var tvShowsLocation = new TVShowsLocation(_fileSystemMock.Object);
            var tvShowsOnDisk = tvShowsLocation.Locate(path, _tvShowFactoryMock.Object, _fileSystemMock.Object);

            Assert.AreEqual(3, tvShowsOnDisk.Count);
        }

        [Test]
        public void DirectoryDoesNotExist()
        {
            const string path = @"C:\TV shows\";

            _fileSystemMock.Setup(fileSystem => fileSystem.DirectoryExists(path)).Returns(false);

            var tvShowsLocation = new TVShowsLocation(_fileSystemMock.Object);

            Assert.That(() => tvShowsLocation.Locate(path, _tvShowFactoryMock.Object, _fileSystemMock.Object), Throws.TypeOf<DirectoryNotFoundException>());
        }
    }
}
