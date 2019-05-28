using System.Collections.Generic;

namespace PerfectCode.TVShowManager.UserInterface.ViewModels.ImportEpisodes
{
    public class ImportedEpisodesOverviewViewModel : ImportEpisodeViewModelBase
    {
        private readonly ImportEpisodesViewModel _importEpisodesViewModel;
        public List<EpisodeDestination> EpisodeDestinations { get; set; }

        public ImportedEpisodesOverviewViewModel(ImportEpisodesViewModel importEpisodesViewModel)
        {
            _importEpisodesViewModel = importEpisodesViewModel;

            DisplayName = "Import episodes overview";
        }

        public override void OnStepActivated()
        {
            base.OnStepActivated();

            EpisodeDestinations = new List<EpisodeDestination>(_importEpisodesViewModel.EpisodeDestinations.Values);
        }
    }
}
