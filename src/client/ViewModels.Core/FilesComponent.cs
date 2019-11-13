using savaged.mvvm.Core.Interfaces;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Windows.Input;

namespace savaged.mvvm.ViewModels.Core
{
    public class FilesComponent<T, F> 
        : DragAndDropComponent<T, F>, IUploadable
        where T : IModelWithUploads, new()
        where F : IFileModel, new()
    {
        public FilesComponent(
            IDragAndDropViewModel owner)
            : base(owner)
        {
            FilesCmd = new RelayCommand<T>(
                OnFiles, (p) => Owner.ViewState.IsNotBusy);
        }

        public ICommand FilesCmd { get; }

        private void OnFiles(T modelWithUploads)
        {
            if (modelWithUploads == null)
            {
                throw new ArgumentNullException(nameof(modelWithUploads));
            }
            var uploadFilesDialogViewModel = new UploadFileDialogViewModel<T, F>(
                Owner.CommonParams, Owner);
            uploadFilesDialogViewModel.Seed(modelWithUploads);

            Owner.NavigationService.DialogService.Show(
                uploadFilesDialogViewModel);
        }

    }
}
