using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.CommandWpf;
using PerfectCode.TVShows;

namespace PerfectCode.TVShowManager.UserInterface.ViewModels.ImportEpisodes
{
    public interface IImportEpisodesViewModel
    {
        Dictionary<string, EpisodeDestination> EpisodeDestinations { get; }
        ImportEpisodeViewModelBase CurrentStep { get; set; }
        bool? DialogResult { get; set; }
        bool WizardInProgress { get; set; }
        bool WizardReady { get; set; }
        ObservableCollection<ImportEpisodeViewModelBase> WizardSteps { get; }
        RelayCommand PreviousCommand { get; }
        RelayCommand NextCommand { get; }
        RelayCommand SkipCommand { get; }
        RelayCommand FinishCommand { get; }

        void SetDownloadedEpisodeDestination(
            DownloadedEpisodeViewModel downloadedEpisode, 
            ITVShow tvShow, 
            ISeason season);

        void Cleanup();
    }
}