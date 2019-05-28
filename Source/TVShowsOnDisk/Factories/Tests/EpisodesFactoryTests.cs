using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using PerfectCode.FileSystemIO;
using PerfectCode.TVShowManager.Application;
using PerfectCode.TVShows;

namespace PerfectCode.TVShowManager.TVShows.Tests
{
    [TestFixture]
    public class EpisodesFactoryTests
    {
        private Mock<IFileSystem> _fileSystemMock;
        private Mock<ISeason> _seasonMock;

        [SetUp]
        public void SetUp()
        {
            _fileSystemMock = new Mock<IFileSystem>();
            _seasonMock = new Mock<ISeason>();
        }

        [TestCase(@"Dexter - S01E01 - Kill kill kill.mkv", 1)]
        [TestCase(@"Dexter - S01E02 - Blooooood.mkv", 2)]
        [TestCase(@"Dexter - S01E05 - Wrap m up.mkv", 5)]
        public void CreateLocalEpisode(string episodeFileName, int expectedEpisodeNumber)
        {
            const string seasonFullPath = @"D:\Dexter\Season 1";
            _seasonMock.Setup(season => season.Path).Returns(seasonFullPath);
            _seasonMock.Setup(season => season.TVShow).Returns(new Mock<ITVShow>().Object);

            _fileSystemMock
                .Setup(fileSystem => fileSystem.DirectoryExists(It.IsAny<string>()))
                .Returns(true);

            _fileSystemMock
                .Setup(fileSystem => fileSystem.EnumerateFiles(It.IsAny<string>(), false, It.IsAny<List<string>>()))
                .Returns(new[] { episodeFileName });

            var episodesFactory = new EpisodesFactory(_fileSystemMock.Object);
            var episodes = episodesFactory.Create(_seasonMock.Object, seasonFullPath, _fileSystemMock.Object);

            Assert.AreEqual(expectedEpisodeNumber, episodes.First().Number);
        }

        [Test]
        public void CreateInvalidLocalEpisode()
        {
            const string seasonFullPath = @"C:\Dexter\Season 1\";
            _fileSystemMock
                .Setup(fileSystem => fileSystem.GetFileName(It.IsAny<string>()))
                .Returns(@"C:\Dexter\Season 1\info.txt");
            var episodesFactory = new EpisodesFactory(_fileSystemMock.Object);
            var episodes = episodesFactory.Create(_seasonMock.Object, seasonFullPath, _fileSystemMock.Object);

            Assert.AreEqual(0, episodes.Count);
        }
    }
}
