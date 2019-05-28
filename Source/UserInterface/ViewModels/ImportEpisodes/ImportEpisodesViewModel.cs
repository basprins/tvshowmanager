using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using PerfectCode.FileSystemIO;
using PerfectCode.TVShows;

namespace PerfectCode.TVShowManager.UserInterface.ViewModels.ImportEpisodes
{
    public class ImportEpisodesViewModel : ViewModelBase, IImportEpisodesViewModel
    {
        private readonly ITVShowLibrary _tvShowLibrary;
        private readonly IFileSystem _fileSystem;
        private ImportEpisodeViewModelBase _currentStep;
        private bool? _dialogResult;
        private bool _wizardInProgress;
        private bool _wizardReady;

        public Dictionary<string, EpisodeDestination> EpisodeDestinations { get; }

        public ImportEpisodeViewModelBase CurrentStep
        {
            get { return _currentStep; }
            set
            {
                Set(() => CurrentStep, ref _currentStep, value);
                CurrentStep.OnStepActivated();
            }
        }

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(() => DialogResult, ref _dialogResult, value); }
        }

        public bool WizardInProgress
        {
            get { return _wizardInProgress; }
            set { Set(() => WizardInProgress, ref _wizardInProgress, value); }
        }

        public bool WizardReady
        {
            get { return _wizardReady; }
            set { Set(() => WizardReady, ref _wizardReady, value); }
        }

        public ObservableCollection<ImportEpisodeViewModelBase> WizardSteps { get; }

        public RelayCommand PreviousCommand { get; }
        public RelayCommand NextCommand { get; }
        public RelayCommand SkipCommand { get; }
        public RelayCommand FinishCommand { get; }

        public ImportEpisodesViewModel(ITVShowLibrary tvShowLibrary, IFileSystem fileSystem)
        {
            _tvShowLibrary = tvShowLibrary;
            _fileSystem = fileSystem;

            EpisodeDestinations = new Dictionary<string, EpisodeDestination>();

            WizardInProgress = true;

            WizardSteps = new ObservableCollection<ImportEpisodeViewModelBase>();

            foreach (var downloadedEpisode in fileSystem.EnumerateFiles(_tvShowLibrary.ImportFolder, true, new List<string> {".mkv", ".avi", ".mp4"}))
            {
                if (!downloadedEpisode.ToLowerInvariant().Contains("sample"))
                {
                    WizardSteps.Add(new DownloadedEpisodeViewModel(downloadedEpisode, tvShowLibrary, this, fileSystem));
                }
            }

            WizardSteps.Add(new ImportedEpisodesOverviewViewModel(this));

            CurrentStep = WizardSteps.First();

            PreviousCommand = new RelayCommand(Previous, CanNavigatePrevious);
            NextCommand = new RelayCommand(Next, CanNavigateNext);
            SkipCommand = new RelayCommand(Skip, CanSkip);
            FinishCommand = new RelayCommand(Finish, CanFinish);
        }

        public void SetDownloadedEpisodeDestination(
            DownloadedEpisodeViewModel downloadedEpisode, 
            ITVShow tvShow, 
            ISeason season)
        {
            EpisodeDestinations[downloadedEpisode.FileName] = new EpisodeDestination(
                downloadedEpisode,
                tvShow,
                season,
                _fileSystem);
        }

        private void Previous()
        {
            var previousStep = WizardSteps.Reverse().SkipWhile(wizardStep => wizardStep != CurrentStep).Skip(1).First();
            CurrentStep = previousStep;
            WizardReady = false;
            WizardInProgress = true;
        }

        private bool CanNavigatePrevious()
        {
            return CurrentStep != WizardSteps.First();
        }

        private void Next()
        {
            CurrentStep.OnExitStep();

            var nextStep = WizardSteps.SkipWhile(wizardStep => wizardStep != CurrentStep).Skip(1).First();
            CurrentStep = nextStep;
        }

        private bool CanNavigateNext()
        {
            var canNavigateNext = CurrentStep != WizardSteps.Last();
            WizardReady = !canNavigateNext;
            WizardInProgress = canNavigateNext;
            var navigateNext = canNavigateNext && CurrentStep.CanNavigateNext();
            return navigateNext;
        }

        private void Skip()
        {
            CurrentStep.OnSkip();

            var nextStep = WizardSteps.SkipWhile(wizardStep => wizardStep != CurrentStep).Skip(1).First();
            CurrentStep = nextStep;
        }

        private bool CanSkip()
        {
            return CurrentStep.CanSkip;
        }


        private void Finish()
        {
            var episodeDestinations = EpisodeDestinations.Values.ToList();

            if (episodeDestinations.Count > 0)
            {
                var importEpisodesProgressViewModel = new ImportEpisodesProgressViewModel(episodeDestinations,
                    _fileSystem);
                var progressDialog = new Views.ProgressDialog
                {
                    DataContext = importEpisodesProgressViewModel,
                    Owner = System.Windows.Application.Current.MainWindow
                };
                importEpisodesProgressViewModel.Import();
                if (progressDialog.ShowDialog() == true)
                {
                    _fileSystem.TryClearDirectory(_tvShowLibrary.ImportFolder);
                }
            }

            DialogResult = true;
        }

        private bool CanFinish()
        {
            return CurrentStep == WizardSteps.Last();
        }
    }
}
