using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using GalaSoft.MvvmLight.Threading;
using Moq;
using NUnit.Framework;
using PerfectCode.FileSystemIO;
using PerfectCode.TvDbRestApi;
using PerfectCode.TVShowManager.Application;
using PerfectCode.TVShows;

namespace PerfectCode.TVShowManager.UserInterface.ViewModels.Tests
{
    [TestFixture]
    public class MainViewModelTests
    {
        private Mock<ITVShowFactory> _tvShowFactoryMock;
        private Mock<ITVShowLibrary> _tvShowLibraryMock;
        private Mock<IDialogService> _dialogServiceMock;
        private Mock<IFolderBrowserDialog> _folderBrowserDialogMock;
        private Mock<IFileSystem> _fileSystemMock;
        private Mock<ITVShowRestClient> _restApi;
        private Mock<ISeasonFactory> _seasonFactoryMock;

        [SetUp]
        public void SetUp()
        {
            _fileSystemMock = new Mock<IFileSystem>();
            _tvShowFactoryMock = new Mock<ITVShowFactory>();
            _tvShowLibraryMock = new Mock<ITVShowLibrary>();
            _dialogServiceMock = new Mock<IDialogService>();
            _folderBrowserDialogMock = new Mock<IFolderBrowserDialog>();
            _restApi = new Mock<ITVShowRestClient>();
            _seasonFactoryMock = new Mock<ISeasonFactory>();

            DispatcherHelper.Initialize();
        }

        [Test]
        public void AddTVShowFromDisk()
        {
            var tvShowMock = new Mock<ITVShow>();
            tvShowMock.Setup(tvShow => tvShow.Seasons).Returns(new List<ISeason>());

            _dialogServiceMock
                .Setup(dialogService => dialogService.NewFolderBrowserDialog(It.IsAny<string>()))
                .Returns(_folderBrowserDialogMock.Object);

            _folderBrowserDialogMock
                .Setup(folderBrowserDialog => folderBrowserDialog.ShowDialog())
                .Returns(DialogResult.OK);
            _folderBrowserDialogMock
                .Setup(folderBrowserDialog => folderBrowserDialog.SelectedPath)
                .Returns(@"C:\tvshow");

            _tvShowFactoryMock
                .Setup(tvShowFactory => tvShowFactory.Create(It.IsAny<string>(), _fileSystemMock.Object))
                .Returns(tvShowMock.Object);

            _tvShowLibraryMock
                .Setup(tvShowLibrary => tvShowLibrary.TVShows)
                .Returns(new List<ITVShow>());

            _tvShowLibraryMock
                .Setup(tvShowLibrary => tvShowLibrary.Add(tvShowMock.Object))
                .Raises(library => library.TVShowAdded += null, new TVShowEventArgs(tvShowMock.Object));

            tvShowMock
                .Setup(tvShow => tvShow.Seasons)
                .Returns(new List<ISeason>() );

            var mainViewModel = new MainViewModel(
                _tvShowFactoryMock.Object, 
                _tvShowLibraryMock.Object, 
                _dialogServiceMock.Object, 
                _fileSystemMock.Object,
                _restApi.Object,
                _seasonFactoryMock.Object);

            mainViewModel.AddTVShowFromDiskCommand.Execute(null);

            Assert.IsNotNull(mainViewModel.TVShows.First());
        }
    }
}
