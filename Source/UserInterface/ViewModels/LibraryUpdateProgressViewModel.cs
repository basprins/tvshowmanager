using System.Linq;
using PerfectCode.TVShowManager.UserInterface.ViewModels.ProgressDialog;

namespace PerfectCode.TVShowManager.UserInterface.ViewModels
{
    public class LibraryUpdateProgressViewModel : ProgressDialogViewModel
    {
        private readonly ITVShowLibrary _tvShowLibrary;

        public LibraryUpdateProgressViewModel(ITVShowLibrary tvShowLibrary)
        {
            _tvShowLibrary = tvShowLibrary;

            Title = "Please wait while updating TV Shows library...";
            Maximum = _tvShowLibrary.TVShows.Count;

            tvShowLibrary.ProgressChanged += OnProgressChanged;
        }

        private void OnProgressChanged(object sender, UpdateTVShowProgressEvent args)
        {
            StatusMessage = $"Currently updating '{args.TVShow.Name}'...";

            if (args.FinishedUpdatingTVShow)
            {
                StatusMessage = $"TV show '{args.TVShow.Name}' is updated";
                Value++;
            }

            if (Value >= Maximum)
            {
                DialogResult = true;
            }
        }
    }
}
