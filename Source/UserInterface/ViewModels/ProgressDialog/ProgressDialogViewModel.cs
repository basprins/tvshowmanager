using GalaSoft.MvvmLight;

namespace PerfectCode.TVShowManager.UserInterface.ViewModels.ProgressDialog
{
    public class ProgressDialogViewModel : ViewModelBase
    {
        private int _maximum;
        private int _value;
        private string _statusMessage;
        private string _title;
        private bool? _dialogResult;

        public int Maximum
        {
            get { return _maximum; }
            set { Set(() => Maximum, ref _maximum, value); }
        }

        public int Value
        {
            get { return _value; }
            set { Set(() => Value, ref _value, value); }
        }

        public string Title
        {
            get { return _title; }
            set { Set(() => Title, ref _title, value); }
        }

        public string StatusMessage
        {
            get { return _statusMessage; }
            set { Set(() => StatusMessage, ref _statusMessage, value); }
        }

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { Set(() => DialogResult, ref _dialogResult, value); }
        }
    }
}
