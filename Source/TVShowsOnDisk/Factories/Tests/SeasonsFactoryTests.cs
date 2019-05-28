using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;
using PerfectCode.FileSystemIO;
using PerfectCode.TVShowManager.Application;
using PerfectCode.TVShows;

namespace PerfectCode.TVShowManager.TVShows.Tests
{
    [TestFixture]
    public class SeasonsFactoryTests
    {
        private Mock<IFileSystem> _fileSystemMock;
        private Mock<ITVShow> _tvShowMock;

        [SetUp]
        public void SetUp()
        {
            _fileSystemMock = new Mock<IFileSystem>();
            _tvShowMock = new Mock<ITVShow>();
        }

        [Test]
        public void CreateSeasonsFromDisk()
        {
            var seasonDirectories = new[]
            {
                @"D:\Dexter\Season 1\",
                @"D:\Dexter\Season 2\",
                @"D:\Dexter\Season 3\"
            };

            _fileSystemMock
                .Setup(fileSystem => fileSystem.EnumerateDirectories(It.IsAny<string>()))
                .Returns(seasonDirectories);

            _fileSystemMock.Setup(fileSystem => fileSystem.GetFileName(It.IsAny<string>()))
                .Returns((string value) => Path.GetFileName(value));

            var seasonsFactory = new SeasonsFactory(_fileSystemMock.Object);
            var seasons = seasonsFactory.Create(_tvShowMock.Object);

            Assert.AreEqual(3, seasons.Count);
        }

        [Test]
        public void CreateInvalidLocalSeason()
        {
            _fileSystemMock
                .Setup(fileSystem => fileSystem.GetLastSubDirectoryName(It.IsAny<string>()))
                .Returns("blabla");

            var seasonsFactory = new SeasonsFactory(_fileSystemMock.Object);
            var seasons = seasonsFactory.Create(_tvShowMock.Object);

            Assert.AreEqual(0, seasons.Count);
        }
    }
}
