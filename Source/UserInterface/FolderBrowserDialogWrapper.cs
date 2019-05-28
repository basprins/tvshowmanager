using System.Windows.Forms;

namespace PerfectCode.TVShowManager.UserInterface
{
    public class FolderBrowserDialogWrapper : IFolderBrowserDialog
    {
        private readonly FolderBrowserDialog _folderBrowserDialog;

        public string SelectedPath => _folderBrowserDialog.SelectedPath;

        public FolderBrowserDialogWrapper(string initialFolder)
        {
            _folderBrowserDialog = new FolderBrowserDialog
            {
                SelectedPath = initialFolder
            };
        }
        
        public DialogResult ShowDialog()
        {
            return _folderBrowserDialog.ShowDialog();
        }
    }
}