namespace PerfectCode.TVShowManager.UserInterface
{
    public class DialogService : IDialogService
    {
        public IFolderBrowserDialog NewFolderBrowserDialog(string initialFolder)
        {
            return new FolderBrowserDialogWrapper(initialFolder);
        }
    }
}
