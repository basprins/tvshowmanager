using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using PerfectCode.FileSystemIO;
using PerfectCode.TVShows;
using StringUtils;

namespace PerfectCode.TVShowManager.UserInterface.ViewModels.ImportEpisodes
{
    public class DownloadedEpisodeViewModel : ImportEpisodeViewModelBase
    {
        private readonly Regex _episodeRegex = new Regex(@"(?<tvShowName>.*)\s?-?\s?S(?<seasonNumber>\d+)E(?<episodeNumber>\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _episode2Regex = new Regex(@"(?<tvShowName>[^\d]*)(?<episodeNumber>\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _tvShowNameRegex = new Regex(@"(\[.*\])?(?<tvShowName>.*)", RegexOptions.Compiled);

        private readonly ITVShowLibrary _tvShowLibrary;
        private readonly IImportEpisodesViewModel _importEpisodesViewModel;
        private ITVShow _selectedTVShow;
        private ISeason _selectedSeason;
        private int? _episodeNumber;

        public string FileName { get; }

        public string CurrentEpisodeFileName { get; }

        public override bool CanSkip => true;

        public ITVShow SelectedTVShow
        {
            get { return _selectedTVShow; }
            set
            {
                Seasons.Clear();

                if (value != null)
                {
                    foreach (var season in value.Seasons.OrderBy(s => s.Number))
                    {
                        Seasons.Add(season);
                    }
                }
                Set(() => SelectedTVShow, ref _selectedTVShow, value);
            }
        }

        public List<ITVShow> TVShows => _tvShowLibrary.TVShows;

        public ISeason SelectedSeason
        {
            get { return _selectedSeason; }
            set { Set(() => SelectedSeason, ref _selectedSeason, value); }
        }

        public int? EpisodeNumber
        {
            get { return _episodeNumber; }
            set { Set(() => EpisodeNumber, ref _episodeNumber, value); }
        }

        public ObservableCollection<ISeason> Seasons { get; } 

        public DownloadedEpisodeViewModel(string fileName, ITVShowLibrary tvShowLibrary, IImportEpisodesViewModel importEpisodesViewModel, IFileSystem fileSystem)
        {
            FileName = fileName;
            _tvShowLibrary = tvShowLibrary;
            _importEpisodesViewModel = importEpisodesViewModel;

            DisplayName = "Import episode";
            CurrentEpisodeFileName = fileSystem.GetFileName(FileName);

            Seasons = new ObservableCollection<ISeason>();

            var match = _episodeRegex.Match(fileSystem.GetFileName(FileName));
            if (match.Success)
            {
                var tvShowName = GetTVShowName(match.Groups["tvShowName"].Value);
                SelectedTVShow = _tvShowLibrary.TVShows.FirstOrDefault(tvShow => tvShow.Name.IsAlike(tvShowName));

                var seasonNumber = int.Parse(match.Groups["seasonNumber"].Value);
                SelectedSeason = Seasons.FirstOrDefault(season => season.Number == seasonNumber);

                EpisodeNumber = int.Parse(match.Groups["episodeNumber"].Value);
            }
            else
            {
                match = _episode2Regex.Match(fileSystem.GetFileName(FileName));
                if (match.Success)
                {
                    var tvShowName = GetTVShowName(match.Groups["tvShowName"].Value);
                    tvShowName = tvShowName.Replace(".", " ");
                    SelectedTVShow = _tvShowLibrary.TVShows.FirstOrDefault(tvShow => tvShow.Name.IsAlike(tvShowName));

                    var episideIdentifier = match.Groups["episodeNumber"].Value;
                    // 103 format
                    if (episideIdentifier.Length > 2 && episideIdentifier.Length <= 3)
                    {
                        var seasonNumber = int.Parse(episideIdentifier.Substring(0, 1));
                        SelectedSeason = Seasons.FirstOrDefault(season => season.Number == seasonNumber);

                        EpisodeNumber = int.Parse(episideIdentifier.Substring(1, 2));
                    }
                    // 1012 format
                    if (episideIdentifier.Length > 3 && episideIdentifier.Length <= 4)
                    {
                        var seasonNumber = int.Parse(episideIdentifier.Substring(0, 2));
                        SelectedSeason = Seasons.FirstOrDefault(season => season.Number == seasonNumber);

                        EpisodeNumber = int.Parse(episideIdentifier.Substring(2, 2));
                    }
                }
            }
        }

        private string GetTVShowName(string tvShowName)
        {
            var match = _tvShowNameRegex.Match(tvShowName);
            if (match.Success)
            {
                return match.Groups["tvShowName"].Value;
            }
            return tvShowName;
        }

        public override bool CanNavigateNext()
        {
            var canNavigateNext = base.CanNavigateNext();
            canNavigateNext &= SelectedTVShow != null && SelectedSeason != null && EpisodeNumber != null;
            return canNavigateNext;
        }

        public override void OnExitStep()
        {
            base.OnExitStep();

            _importEpisodesViewModel.SetDownloadedEpisodeDestination(this, SelectedTVShow, SelectedSeason);
        }

        public override void OnSkip()
        {
            base.OnSkip();

            IsSkipped = true;
        }

        public bool IsSkipped { get; private set; }
    }
}