using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.CommandWpf;
using PerfectCode.TVShows;

namespace PerfectCode.TVShowManager.UserInterface.ViewModels
{
    public interface ITVShowViewModel
    {
        ITVShow TVShow { get; }
        string Name { get; set; }
        SeasonViewModel SelectedSeason { get; set; }
        bool DownloadsAvailable { get; set; }
        bool HasNewEpisodes { get; set; }
        bool ComingSoon { get; set; }
        int NumberOfSeasons { get; set; }
        string Path { get; }
        ObservableCollection<SeasonViewModel> Seasons { get; }
        RelayCommand RemoveTVShowCommand { get; }
    }
}