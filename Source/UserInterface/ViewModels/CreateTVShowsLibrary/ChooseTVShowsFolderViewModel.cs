using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using GalaSoft.MvvmLight.CommandWpf;
using PerfectCode.FileSystemIO;
using PerfectCode.TVShowManager.Application;
using PerfectCode.TVShows;

namespace PerfectCode.TVShowManager.UserInterface.ViewModels.CreateTVShowsLibrary
{
    public class ChooseTVShowsFolderViewModel : WizardPageViewModel
    {
        private readonly ICreateTVShowsLibraryWizardViewModel _parent;
        private readonly ITVShowLocation _tvShowsLocation;
        private readonly IDialogService _dialogService;
        private readonly IFileSystem _fileSystem;
        private readonly ITVShowLibrary _tvShowLibrary;
        private string _selectedPath;

        public string SelectedPath
        {
            get { return _selectedPath; }
            set { Set(() => SelectedPath, ref _selectedPath, value); }
        }

        public ObservableCollection<TVShowViewModel> TVShows { get; }
        public RelayCommand ChooseTVFolderCommand { get; }

        public ChooseTVShowsFolderViewModel(ICreateTVShowsLibraryWizardViewModel parent, ITVShowLocation tvShowsLocation, IDialogService dialogService, IFileSystem fileSystem, ITVShowLibrary tvShowLibrary)
        {
            _parent = parent;
            _tvShowsLocation = tvShowsLocation;
            _dialogService = dialogService;
            _fileSystem = fileSystem;
            _tvShowLibrary = tvShowLibrary;

            DisplayName = "Choose TV shows folder";
            TVShows = new ObservableCollection<TVShowViewModel>();

            ChooseTVFolderCommand = new RelayCommand(ChooseTVFolder);
        }

        public override bool CanNext()
        {
            return true;
        }

        private void ChooseTVFolder()
        {
            var folderDialog = _dialogService.NewFolderBrowserDialog(_tvShowLibrary.TVShowLocationPath);
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                SelectedPath = folderDialog.SelectedPath;

                var tvShows = _tvShowsLocation.Locate(folderDialog.SelectedPath, new TVShowFactory(), _fileSystem);

                TVShows.Clear();
                foreach (var tvShow in tvShows)
                {
                    TVShows.Add(new TVShowViewModel(tvShow, _tvShowLibrary));
                }

                var pages = TVShows.Select(tvShowViewModel => tvShowViewModel.TVShow).ToList();
                _parent.SetTVShowPages(pages);
                _parent.SelectedTVShowsFolderPath = SelectedPath;
            }
        }
    }
}
