using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using PerfectCode.FileSystemIO;
using PerfectCode.TvDbRestApi;
using PerfectCode.TVShowManager.UserInterface.ViewModels.CreateTVShowsLibrary;
using PerfectCode.TVShows;

namespace PerfectCode.TVShowManager.UserInterface.ViewModels.AddSingleTVShowFromDisk
{
    public class AddTVShowFromDiskViewModel : ViewModelBase
    {
        private readonly IFileSystem _fileSystem;
        private readonly ITVShowLibrary _tvShowLibrary;
        private readonly ITVShowRestClient _tvShowRestClient;
        private readonly IDialogService _dialogService;

        private string _selectedPath;
        private CandidateTVShowViewModel _selectedCandidate;
        private bool? _dialogResult;

        public string SelectedPath
        {
            get { return _selectedPath; }
            set { Set(() => SelectedPath, ref _selectedPath, value); }
        }

        public CandidateTVShowViewModel SelectedCandidate
        {
            get { return _selectedCandidate; }
            set { Set(() => SelectedCandidate, ref _selectedCandidate, value); }
        }

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(() => DialogResult, ref _dialogResult, value); }
        }

        public ObservableCollection<CandidateTVShowViewModel> Candidates { get; }

        public RelayCommand SelectTVShowFolderCommand { get; }
        public RelayCommand OKCommand { get; }

        public AddTVShowFromDiskViewModel(
            ITVShowLibrary tvShowLibrary, 
            ITVShowRestClient tvShowRestClient, 
            IFileSystem fileSystem, 
            IDialogService dialogService)
        {
            _fileSystem = fileSystem;
            _tvShowLibrary = tvShowLibrary;
            _tvShowRestClient = tvShowRestClient;
            _dialogService = dialogService;

            Candidates = new ObservableCollection<CandidateTVShowViewModel>();

            SelectTVShowFolderCommand = new RelayCommand(SelectTVShowFolder);
            OKCommand = new RelayCommand(OK, CanOK);
        }

        private void OK()
        {
            DialogResult = true;
        }

        private bool CanOK()
        {
            return SelectedCandidate != null;
        }

        private void SelectTVShowFolder()
        {
            var folderDialog = _dialogService.NewFolderBrowserDialog(_tvShowLibrary.TVShowLocationPath);

            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SelectedPath = folderDialog.SelectedPath;

                ShowCandidates();
            }
        }

        private async void ShowCandidates()
        {
            Candidates.Clear();

            var restApiTVShows = await _tvShowRestClient.GetTvShowsByName(_fileSystem.GetLastSubDirectoryName(SelectedPath));
            foreach (var restApiTVShow in restApiTVShows)
            {
                var posterUrl = await _tvShowRestClient.GetPosterImageUrl(restApiTVShow.PosterPath);
                Candidates.Add(new CandidateTVShowViewModel(restApiTVShow, posterUrl));
            }

            SelectedCandidate = Candidates.FirstOrDefault();

        }
    }
}
