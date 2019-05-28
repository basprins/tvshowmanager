using System.Collections.Generic;
using System.IO;
using Moq;
using NUnit.Framework;
using PerfectCode.FileSystemIO;
using PerfectCode.TVShowManager.UserInterface.ViewModels.ImportEpisodes;
using PerfectCode.TVShows;

namespace PerfectCode.TVShowManager.Source.UserInterface.ViewModels.ImportEpisodes.Tests
{
    [TestFixture]
    public class DownloadedEpisodeViewModelTests
    {
        private Mock<ITVShowLibrary> _tvShowsLibraryMock;
        private Mock<IImportEpisodesViewModel> _importEpisodesViewModelMock;
        private Mock<IFileSystem> _fileSystemMock;

        [SetUp]
        public void SetUp()
        {
            _tvShowsLibraryMock = new Mock<ITVShowLibrary>();
            _importEpisodesViewModelMock = new Mock<IImportEpisodesViewModel>();
            _fileSystemMock = new Mock<IFileSystem>();
        }

        [TestCase(@"D:\Movies\Dexter - S01E23.hdtv-lol[ettv].mp4", "Dexter", 1, 23)]
        [TestCase(@"D:\Movies\how.to.get.away.with.murder.214.hdtv-lol[ettv].mp4", "how to get away with murder", 2, 14)]
        [TestCase(@"D:\Movies\greys anatomy 1014.hdtv-lol[ettv].mp4", "greys anatomy", 10, 14)]
        public void ReconstructEpisode(string fileName, string tvShowName, int expectedSeason, int expectedEpisode)
        {
            _fileSystemMock.Setup(fileSystem => fileSystem.GetFileName(fileName)).Returns(Path.GetFileName(fileName));

            var seasonMock = new Mock<ISeason>();
            seasonMock.Setup(season => season.Number).Returns(expectedSeason);

            var tvShowMock = new Mock<ITVShow>();
            tvShowMock.Setup(tvShow => tvShow.Name).Returns(tvShowName);
            tvShowMock.Setup(tvShow => tvShow.Seasons).Returns(new List<ISeason> {seasonMock.Object});

            _tvShowsLibraryMock.Setup(library => library.TVShows).Returns(new List<ITVShow>{tvShowMock.Object});

            var downloadedEpisode = new DownloadedEpisodeViewModel(
                fileName, 
                _tvShowsLibraryMock.Object, 
                _importEpisodesViewModelMock.Object, 
                _fileSystemMock.Object);

            Assert.AreEqual(expectedSeason, downloadedEpisode.SelectedSeason.Number);
            Assert.AreEqual(expectedEpisode, downloadedEpisode.EpisodeNumber);
        }
    }
}
