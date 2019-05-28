using Moq;
using NUnit.Framework;
using PerfectCode.FileSystemIO;
using PerfectCode.TVShowManager.Application;
using PerfectCode.TVShows;

namespace PerfectCode.TVShowManager.TVShows.Tests
{
    [TestFixture]
    public class TVShowFactoryTests
    {
        private Mock<IFileSystem> _fileSystemMock;
        private Mock<ISeasonFactory> _seasonFactoryMock;

        [SetUp]
        public void SetUp()
        {
            _fileSystemMock = new Mock<IFileSystem>();
            _seasonFactoryMock = new Mock<ISeasonFactory>();
        }

        [Test]
        public void CreateTVShowFromDisk()
        {
            _fileSystemMock
                .Setup(fileSystem => fileSystem.GetLastSubDirectoryName(It.IsAny<string>()))
                .Returns(@"Dexter");

            var tvShowFactory = new TVShowFactory();
            var tvShow = tvShowFactory.Create(@"D:\Dexter", _fileSystemMock.Object);
            tvShow.CheckFileSystem(_seasonFactoryMock.Object);

            _seasonFactoryMock.Verify(seasonFactory => seasonFactory.Create(It.IsAny<ITVShow>()));
            Assert.AreEqual("Dexter", tvShow.Name);
        }
    }
}
