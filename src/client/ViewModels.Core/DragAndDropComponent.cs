using savaged.mvvm.Core.Interfaces;
using savaged.mvvm.ViewModels.Core.Utils;
using System;
using System.Windows;

namespace savaged.mvvm.ViewModels.Core
{
    public class DragAndDropComponent<T, F> : IDragAndDropable
        where T : IModelWithUploads, new()
        where F : IFileModel, new()
    {
        public DragAndDropComponent(
            IDragAndDropViewModel owner)
        {
            Owner = owner;
        }

        protected IDragAndDropViewModel Owner { get; }

        public void OnDrop(object sender, DragEventArgs e)
        {
            var modelWithUploads = 
                DragDropHelper.DraggedItemToModelWithUploads(e);

            if (!(modelWithUploads is T typedModelWithUploads))
                throw new InvalidOperationException(
                    "The dragged item is not of the required type " +
                    typeof(T).Name);

            var uploadFilesDialogViewModel =
                new UploadFileDialogViewModel<T, F>(
                    Owner.CommonParams, Owner, true);
            uploadFilesDialogViewModel.Seed(typedModelWithUploads);

            uploadFilesDialogViewModel.OnDrop(sender, e);

            Owner.NavigationService.DialogService.Show(
                uploadFilesDialogViewModel);
        }
        
    }
}
