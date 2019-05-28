using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using PerfectCode.TVShows;

namespace PerfectCode.TVShowManager.UserInterface.ViewModels
{
    public class EpisodeViewModel : ViewModelBase
    {
        private string _name;
        private DateTime _airDate;
        private bool _isPresentOnDisk;
        private bool _isNew;
        private bool _canDownload;
        private bool _comingSoon;
        private bool _isSelected;

        public IEpisode Episode { get; }

        public int Number => Episode.Number;

        public bool IsSelected
        {
            get { return _isSelected; }
            set { Set(() => IsSelected, ref _isSelected, value); }
        }

        public bool IsPresentOnDisk
        {
            get { return _isPresentOnDisk; }
            set { Set(() => IsPresentOnDisk, ref _isPresentOnDisk, value); }
        }

        public bool IsNew
        {
            get { return _isNew; }
            set { Set(() => IsNew, ref _isNew, value); }
        }

        public bool CanDownload
        {
            get { return _canDownload; }
            set { Set(() => CanDownload, ref _canDownload, value); }
        }

        public bool ComingSoon
        {
            get { return _comingSoon; }
            set { Set(() => ComingSoon, ref _comingSoon, value); }
        }

        public string Name
        {
            get { return _name; }
            set { Set(() => Name, ref _name, value); }
        }

        public DateTime AirDate
        {
            get { return _airDate; }
            set { Set(() => AirDate, ref _airDate, value); }
        }

        public RelayCommand OpenInExplorerCommand { get; }
        public RelayCommand MarkAsWatchedCommand { get; }
        public RelayCommand MarkAsUnwatchedCommand { get; }

        public EpisodeViewModel(IEpisode episode)
        {
            Episode = episode;
            Name = episode.Name;

            OpenInExplorerCommand = new RelayCommand(OpenInExplorer);
            MarkAsWatchedCommand = new RelayCommand(MarkAsWatched, CanMarkAsWatched);
            MarkAsUnwatchedCommand = new RelayCommand(MarkAsUnwatched, CanMarkAsUnwatched);

            Episode.PropertyChanged += EpisodeOnPropertyChanged;
        }

        public override string ToString()
        {
            return $"E{Number:00} '{Name}'";
        }

        private void EpisodeOnPropertyChanged(object sender, EventArgs eventArgs)
        {
            Name = Episode.Name;
            AirDate = Episode.AirDate;
            CanDownload = Episode.CanDownload;
            IsPresentOnDisk = Episode.IsPresentOnDisk;
            ComingSoon = Episode.ComingSoon;
            IsNew = Episode.IsNew && !Episode.CanDownload && !Episode.ComingSoon;
        }

        private void OpenInExplorer()
        {
            Episode.OpenInExplorer();
        }

        private void MarkAsWatched()
        {
            Episode.MarkAsWatched();
        }

        private bool CanMarkAsWatched()
        {
            return IsNew;
        }

        private void MarkAsUnwatched()
        {
            Episode.MarkAsUnwatched();
        }

        private bool CanMarkAsUnwatched()
        {
            return !IsNew;
        }
    }
}