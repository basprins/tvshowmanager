using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Threading;
using Moq;
using NUnit.Framework;
using PerfectCode.TVShowManager.UserInterface.ViewModels;
using PerfectCode.TVShows;

namespace PerfectCode.TVShowManager.TV_Show_Manager.UserInterface.ViewModels.Tests
{
    [TestFixture]
    public class TVShowViewModelTests
    {
        private Mock<ISeason> _seasonMock;
        private Mock<ITVShow> _tShowMock;
        private Mock<ITVShowLibrary> _tvShowLibraryMock;

        [SetUp]
        public void SetUp()
        {
            DispatcherHelper.Initialize();

            _tvShowLibraryMock = new Mock<ITVShowLibrary>();
            _seasonMock = new Mock<ISeason>();
            _tShowMock = new Mock<ITVShow>();
        }

        [Test]
        public void SelectLastSeason()
        {
            _seasonMock.Setup(season => season.Number).Returns(1);
            _seasonMock.Setup(season => season.Episodes).Returns(new List<IEpisode>());
            _tShowMock.Setup(tvShow => tvShow.Seasons).Returns(new List<ISeason> {_seasonMock.Object});

            var tvShowViewModel = new TVShowViewModel(_tShowMock.Object, _tvShowLibraryMock.Object);

            Assert.AreEqual(_seasonMock.Object, tvShowViewModel.Seasons.Last().Season);
        }

        /// <summary>
        /// Bugfix: first set selected season, then sort the seasons => last season wasn't selected in UI.
        /// Test if the selected season is set accordingly, regardless of the order in the domain seasons.
        /// </summary>
        [Test]
        public void SelectLastSeasonAfterUpdateFromInternet()
        {
            _seasonMock.Setup(season => season.Number).Returns(1);
            _seasonMock.Setup(season => season.Episodes).Returns(new List<IEpisode>());

            var season2Mock = new Mock<ISeason>();
            season2Mock.Setup(season => season.Number).Returns(2);
            season2Mock.Setup(season => season.Episodes).Returns(new List<IEpisode>());

            var season3Mock = new Mock<ISeason>();
            season3Mock.Setup(season => season.Number).Returns(3);
            season3Mock.Setup(season => season.Episodes).Returns(new List<IEpisode>());

            _tShowMock
                .Setup(tvShow => tvShow.Seasons)
                .Returns(new List<ISeason> {_seasonMock.Object});

            var tvShowViewModel = new TVShowViewModel(_tShowMock.Object, _tvShowLibraryMock.Object);

            _tShowMock
                .Setup(tvShow => tvShow.Seasons)
                .Returns(new List<ISeason> { _seasonMock.Object, season3Mock.Object, season2Mock.Object });

            _tShowMock.Raise(tvShow => tvShow.PropertyChanged += null, new EventArgs());

            Assert.AreEqual(3, tvShowViewModel.SelectedSeason.Season.Number);
        }
    }
}
