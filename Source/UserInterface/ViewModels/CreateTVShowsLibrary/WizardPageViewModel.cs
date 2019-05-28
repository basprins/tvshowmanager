using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace PerfectCode.TVShowManager.UserInterface.ViewModels.CreateTVShowsLibrary
{
    public class WizardPageViewModel : ViewModelBase
    {
        public string DisplayName { get; set; }
        public bool Skipped { get; set; }

        public virtual bool CanNext()
        {
            return true;
        }

        public virtual void OnNextExecuted()
        {
        }

        public virtual async Task OnPageActivated()
        {
            await Task.Run(() => { });
        }
    }
}
