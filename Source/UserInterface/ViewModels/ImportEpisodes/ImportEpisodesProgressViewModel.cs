using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PerfectCode.FileSystemIO;
using PerfectCode.TVShowManager.UserInterface.ViewModels.ProgressDialog;

namespace PerfectCode.TVShowManager.UserInterface.ViewModels.ImportEpisodes
{
    public class ImportEpisodesProgressViewModel : ProgressDialogViewModel
    {
        private readonly List<EpisodeDestination> _episodeDestinations;
        private readonly IFileSystem _fileSystem;

        public ImportEpisodesProgressViewModel(List<EpisodeDestination> episodeDestinations, IFileSystem fileSystem)
        {
            _episodeDestinations = episodeDestinations;
            _fileSystem = fileSystem;

            Title = "Please wait while importing new episodes...";
            Maximum = _episodeDestinations.Count(episode => !episode.IsSkipped);
        }

        public void Import()
        {
            Task.Factory.StartNew(() =>
            {
                foreach (var episodeDestination in _episodeDestinations.Where(episode => !episode.IsSkipped))
                {
                    try
                    {
                        StatusMessage = $"Importing '{_fileSystem.GetFileName(episodeDestination.TargetFileName)}'";

                        _fileSystem.EnsureDirectory(episodeDestination.TargetFileName);
                        _fileSystem.CopyFile(episodeDestination.OriginalFileName, episodeDestination.TargetFileName);

                        var correspondingEpsisode = episodeDestination.Season.Episodes
                            .FirstOrDefault(episode => episode.Number == episodeDestination.EpisodeNumber);

                        if (correspondingEpsisode != null)
                        {
                            correspondingEpsisode.EpisodeFullName = episodeDestination.TargetFileName;
                        }
                    }
                    catch (Exception)
                    {
                        // Ignore exceptions while copying files (if it failed, it failed)
                    }
                    Value++;

                    if (Value >= Maximum)
                    {
                        DialogResult = true;
                    }
                }
            });
        }
    }
}