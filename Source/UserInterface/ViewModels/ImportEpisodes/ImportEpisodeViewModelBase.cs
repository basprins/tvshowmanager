using GalaSoft.MvvmLight;

namespace PerfectCode.TVShowManager.UserInterface.ViewModels.ImportEpisodes
{
    public abstract class ImportEpisodeViewModelBase : ViewModelBase
    {
        public string DisplayName { get; set; }

        public virtual bool CanSkip => false;

        public virtual void OnStepActivated()
        {
            
        }

        public virtual bool CanNavigateNext()
        {
            return true;
        }

        public virtual void OnExitStep()
        {
        }

        public virtual void OnSkip()
        {
        }
    }
}