using System;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using PerfectCode.TvDbRestApi;

namespace PerfectCode.TVShowManager.UserInterface.ViewModels.CreateTVShowsLibrary
{
    public class CandidateTVShowViewModel : ViewModelBase
    {
        private readonly string _posterPath;
        private bool _isSelected;

        public IRestApiTVShow RestTVShow { get; }

        public int WebID => RestTVShow.Id;
        public string Name => RestTVShow.Name;
        public string FirstAirDate => RestTVShow.FirstAirDate;
        public double Average => RestTVShow.VoteAverage;
        public int NumberOfVotes => RestTVShow.VoteCount;

        public bool IsSelected
        {
            get { return _isSelected; }
            set { Set(() => IsSelected, ref _isSelected, value); }
        }

        public string Overview => RestTVShow.Overview;

        public BitmapImage Image
        {
            get
            {
                var bitmapImage = new BitmapImage(new Uri(_posterPath));
                return bitmapImage;
            }
        }

        public CandidateTVShowViewModel(IRestApiTVShow restTVShow, string posterPath)
        {
            _posterPath = posterPath;
            RestTVShow = restTVShow;
        }
    }
}
