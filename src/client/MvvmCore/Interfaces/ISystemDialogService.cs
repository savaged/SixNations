namespace savaged.mvvm.Core.Interfaces
{
    public interface ISystemDialogService
    {
        string ShowOpenFileDialog(
                string title = null,
                string initialDirectory = null,
                string fileFilter = null,
                string filename = null);
    }
}
