using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using PerfectCode.FileSystemIO;

namespace PerfectCode.TVShowManager.UserInterface.ViewModels.Settings
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly IFileSystem _fileSystem;
        private string _selectedImportFolderPath;
        private bool? _dialogResult;
        private string _selectedTVShowsFolderPath;

        public string SelectedImportFolderPath
        {
            get { return _selectedImportFolderPath; }
            set { Set(() => SelectedImportFolderPath, ref _selectedImportFolderPath, value); }
        }

        public string SelectedTVShowsFolderPath
        {
            get { return _selectedTVShowsFolderPath; }
            set { Set(() => SelectedTVShowsFolderPath, ref _selectedTVShowsFolderPath, value); }
        }

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(() => DialogResult, ref _dialogResult, value); }
        }

        public RelayCommand SelectImportFolderCommand { get; }
        public RelayCommand SelectTVShowsFolderCommand { get; }

        public RelayCommand OKCommand { get; }
        public RelayCommand CancelCommand { get; }

        public SettingsViewModel(IDialogService dialogService, IFileSystem fileSystem, ITVShowLibrary tvShowLibrary)
        {
            _dialogService = dialogService;
            _fileSystem = fileSystem;

            SelectedImportFolderPath = tvShowLibrary.ImportFolder;
            SelectedTVShowsFolderPath = tvShowLibrary.TVShowLocationPath;

            SelectImportFolderCommand = new RelayCommand(SelectImportFolder);
            SelectTVShowsFolderCommand = new RelayCommand(SelectTVShowsFolder);
            OKCommand = new RelayCommand(OK, CanOK);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void SelectImportFolder()
        {
            var folderBrowserDialog = _dialogService.NewFolderBrowserDialog(@"");
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SelectedImportFolderPath = folderBrowserDialog.SelectedPath;
            }
        }

        private void SelectTVShowsFolder()
        {
            var folderBrowserDialog = _dialogService.NewFolderBrowserDialog(@"");
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SelectedTVShowsFolderPath = folderBrowserDialog.SelectedPath;
            }
        }

        private void OK()
        {
            DialogResult = true;
        }

        private bool CanOK()
        {
            return _fileSystem.DirectoryExists(SelectedImportFolderPath);
        }

        private void Cancel()
        {
            DialogResult = false;
        }
    }
}
