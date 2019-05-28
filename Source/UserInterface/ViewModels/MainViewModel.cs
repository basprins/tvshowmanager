using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using PerfectCode.FileSystemIO;
using PerfectCode.TvDbRestApi;
using PerfectCode.TVShowManager.Application;
using PerfectCode.TVShowManager.UserInterface.ViewModels.AddSingleTVShowFromDisk;
using PerfectCode.TVShowManager.UserInterface.ViewModels.AddSingleTVShowFromWeb;
using PerfectCode.TVShowManager.UserInterface.ViewModels.CreateTVShowsLibrary;
using PerfectCode.TVShowManager.UserInterface.ViewModels.ImportEpisodes;
using PerfectCode.TVShowManager.UserInterface.ViewModels.Settings;
using PerfectCode.TVShowManager.UserInterface.Views;
using PerfectCode.TVShows;

namespace PerfectCode.TVShowManager.UserInterface.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ITVShowFactory _tvShowFactory;
        private readonly ITVShowLibrary _tvShowLibrary;
        private readonly IDialogService _dialogService;
        private readonly IFileSystem _fileSystem;
        private readonly ITVShowRestClient _restApi;
        private readonly ISeasonFactory _seasonFactory;
        private ITVShowViewModel _selectedTVShow;

        public RelayCommand CreateTVShowsLibraryCommand { get; }
        public RelayCommand AddTVShowFromDiskCommand { get; }
        public RelayCommand AddTVShowFromWebCommand { get; }
        public RelayCommand ImportEpisodesCommand { get; }
        public RelayCommand SettingsCommand { get; }
        public RelayCommand AboutCommand { get; }

        public ObservableCollection<ITVShowViewModel> TVShows { get; }

        public ITVShowViewModel SelectedTVShow
        {
            get => _selectedTVShow;
            set
            {
                Set(() => SelectedTVShow, ref _selectedTVShow, value);

                if (value != null)
                {
                    _tvShowLibrary.UpdateTvShow(value.TVShow);
                }
            }
        }

        public MainViewModel(
            ITVShowFactory tvShowFactory,
            ITVShowLibrary tvShowLibrary,
            IDialogService dialogService,
            IFileSystem fileSystem,
            ITVShowRestClient restApi,
            ISeasonFactory seasonFactory)
        {
            _tvShowFactory = tvShowFactory;
            _tvShowLibrary = tvShowLibrary;
            _dialogService = dialogService;
            _fileSystem = fileSystem;
            _restApi = restApi;
            _seasonFactory = seasonFactory;

            CreateTVShowsLibraryCommand = new RelayCommand(CreateTVShowsLibrary);
            AddTVShowFromDiskCommand = new RelayCommand(AddTVShowFromDisk, CanAddTVShows);
            AddTVShowFromWebCommand = new RelayCommand(AddTVShowFromWeb, CanAddTVShows);
            ImportEpisodesCommand = new RelayCommand(ImportDownloadedEpisodes, CanImportEpisodes);
            SettingsCommand = new RelayCommand(OpenSettings);
            AboutCommand = new RelayCommand(About);

            TVShows = new ObservableCollection<ITVShowViewModel>();

            foreach (var tvShow in _tvShowLibrary.TVShows)
            {
                TVShows.Add(new TVShowViewModel(tvShow, _tvShowLibrary));
            }

            SortTVShows();

            _tvShowLibrary.TVShowAdded += OnTVShowAdded;
            _tvShowLibrary.TVShowRemoved += OnTVShowRemoved;
            _tvShowLibrary.TVShowLibraryUpdateStarted += OnTVShowLibraryUpdateStarted;
        }

        private void CreateTVShowsLibrary()
        {
            var libraryWizardViewModel = new CreateTVShowsLibraryWizardViewModel(_fileSystem, _dialogService, _restApi, _tvShowLibrary);
            var createTVShowLibraryDialog = new CreateTVShowsLibraryDialog
            {
                Owner = System.Windows.Application.Current.MainWindow,
                DataContext = libraryWizardViewModel
            };

            if (createTVShowLibraryDialog.ShowDialog() == true)
            {
                _tvShowLibrary.TVShowLocationPath = libraryWizardViewModel.SelectedTVShowsFolderPath;
                _tvShowLibrary.AddMany(libraryWizardViewModel.MatchedTVShows.Keys.ToList());
            }
        }

        private void AddTVShowFromDisk()
        {
            var addTVShowFromDiskViewModel = new AddTVShowFromDiskViewModel(
                _tvShowLibrary,
                _restApi,
                _fileSystem,
                _dialogService);

            var addTVShowFromDiskDialog = new AddSingleTVShowFromDiskDialog
            {
                Owner = System.Windows.Application.Current.MainWindow,
                DataContext = addTVShowFromDiskViewModel
            };

            if (addTVShowFromDiskDialog.ShowDialog() == true)
            {
                var tvShowOnDisk = new TVShow(addTVShowFromDiskViewModel.SelectedPath, _fileSystem)
                {
                    WebID = addTVShowFromDiskViewModel.SelectedCandidate.WebID
                };

                _tvShowLibrary.Add(tvShowOnDisk);
                tvShowOnDisk.CheckFileSystem(_seasonFactory);
                Task.Factory.StartNew(() => tvShowOnDisk.CheckForUpdates(_restApi));
            }
        }

        private void AddTVShowFromWeb()
        {
            var addTVShowFromWebViewModel = new AddTVShowFromWebViewModel(_restApi);
            var addTVShowFromWebDialog = new AddTVShowFromWebDialog
            {
                Owner = System.Windows.Application.Current.MainWindow,
                DataContext = addTVShowFromWebViewModel
            };

            if (addTVShowFromWebDialog.ShowDialog() == true)
            {
                var restApiTVShow = addTVShowFromWebViewModel.NewTVShow;

                var fullTVShowPath = _fileSystem.CreateDirectory(_tvShowLibrary.TVShowLocationPath, restApiTVShow.Name);
                var tvShow = new TVShow(fullTVShowPath, _fileSystem) {WebID = restApiTVShow.Id};
                _tvShowLibrary.Add(tvShow);
                tvShow.CheckFileSystem(_seasonFactory);
                Task.Factory.StartNew(() => tvShow.CheckForUpdates(_restApi));
            }
        }

        private bool CanAddTVShows()
        {
            return _fileSystem.DirectoryExists(_tvShowLibrary.TVShowLocationPath);
        }

        private void ImportDownloadedEpisodes()
        {
            var wizardViewModel = new ImportEpisodesViewModel(_tvShowLibrary, _fileSystem);
            var importEpisodesWizard = new ImportEpisodesDialog
            {
                Owner = System.Windows.Application.Current.MainWindow,
                DataContext = wizardViewModel
            };

            if (importEpisodesWizard.ShowDialog() == true)
            {

            }
        }

        private bool CanImportEpisodes()
        {
            return _tvShowLibrary.TVShows.Any();
        }

        private void OpenSettings()
        {
            var settingsViewModel = new SettingsViewModel(_dialogService, _fileSystem, _tvShowLibrary);
            var settingsDialog = new SettingsDialog
            {
                Owner = System.Windows.Application.Current.MainWindow,
                DataContext = settingsViewModel
            };

            if (settingsDialog.ShowDialog() == true)
            {
                _tvShowLibrary.ImportFolder = settingsViewModel.SelectedImportFolderPath;
                _tvShowLibrary.TVShowLocationPath = settingsViewModel.SelectedTVShowsFolderPath;
                _tvShowLibrary.Store();
            }
        }

        private void About()
        {
            var aboutViewModel = new AboutViewModel();
            var aboutWindow = new AboutWindow
            {
                Owner = System.Windows.Application.Current.MainWindow,
                DataContext = aboutViewModel
            };

            if (aboutWindow.ShowDialog() == true)
            {
            }
        }

        private void OnTVShowAdded(object sender, TVShowEventArgs args)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                TVShows.Add(new TVShowViewModel(args.TVShow, _tvShowLibrary));
                SortTVShows();
            });
        }

        private void OnTVShowRemoved(object sender, TVShowEventArgs args)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                var tvShowViewModel = TVShows.FirstOrDefault(viewModel => viewModel.Name == args.TVShow.Name);
                if (tvShowViewModel != null)
                {
                    TVShows.Remove(tvShowViewModel);
                }
            });
        }

        private void OnTVShowLibraryUpdateStarted(object sender, EventArgs args)
        {
            if (_tvShowLibrary.TVShows.Any())
            {
                var resetEvent = new ManualResetEvent(false);
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    var progressBarViewModel = new LibraryUpdateProgressViewModel(_tvShowLibrary);
                    var progressDialog = new Views.ProgressDialog
                    {
                        DataContext = progressBarViewModel,
                        Owner = System.Windows.Application.Current.MainWindow
                    };
                    resetEvent.Set();
                    progressDialog.ShowDialog();
                });
                // Block the calling thread for at least until the dialog and its viewmodel (and thus their subscriptions) are created
                resetEvent.WaitOne(1000);
            }
        }

        private void SortTVShows()
        {
            var orderedTvShows = TVShows.OrderBy(tvShow => tvShow.Name).ToList();
            TVShows.Clear();
            foreach (var tvShowViewModel in orderedTvShows)
            {
                TVShows.Add(tvShowViewModel);
            }
        }
    }
}
