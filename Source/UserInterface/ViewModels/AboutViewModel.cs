using System.Diagnostics;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace PerfectCode.TVShowManager.UserInterface.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        private bool? _dialogResult;

        public string Homepage => "https://sourceforge.net/projects/tv-shows";

        public string Version => "1.1.13";

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(() => DialogResult, ref _dialogResult, value); }
        }

        public RelayCommand ShowHomepageCommand { get; }
        public RelayCommand CloseCommand { get; }

        public AboutViewModel()
        {
            ShowHomepageCommand = new RelayCommand(ShowHomepage);
            CloseCommand = new RelayCommand(Close);

        }

        private void ShowHomepage()
        {
            Process.Start(Homepage);
        }

        private void Close()
        {
            DialogResult = true;
        }
    }
}
