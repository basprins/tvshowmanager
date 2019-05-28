using System.Windows.Forms;

namespace PerfectCode.TVShowManager.UserInterface
{
    public interface IFolderBrowserDialog
    {
        string SelectedPath { get; }
        DialogResult ShowDialog();
    }
}