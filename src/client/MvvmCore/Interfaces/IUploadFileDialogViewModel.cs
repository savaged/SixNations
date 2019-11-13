using System.Windows.Input;

namespace savaged.mvvm.Core.Interfaces
{
    public interface IUploadFileDialogViewModel<R, F>
        : IDualModeDialogViewModel<F>, IUploading<R, F>, IDragAndDropViewModel
        where R : IModelWithUploads, new()
        where F : IFileModel, new()
    {
        bool CanSave { get; }
        bool CanShowUploadedFiles { get; }
        string DownloadLocation { get; set; }
        ICommand DownloadUploadedFileCmd { get; }        
        ICommand ShowUploadedFileCmd { get; }

        bool ConfirmDelete(string additionalMsg = null, bool isArchiveOnly = false);
    }
}