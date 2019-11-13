using savaged.mvvm.Core.Interfaces;
using savaged.mvvm.Navigation;
using savaged.mvvm.ViewModels.Core.Utils.Messages;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace savaged.mvvm.ViewModels.Core
{
    public class SelectedItemWithUploadsViewModel<T, F>
        : SelectedItemViewModel<T>, ISelectedItemWithUploadsViewModel<T, F>
        where T : IModelWithUploads, new()
        where F : IFileModel, new()
    {
        private readonly IUploadable _uploadableComponent;

        public SelectedItemWithUploadsViewModel(
            IViewModelCommonParams commonParams,
            IOwnedFocusable owner = null,
            IModelObjectDialogViewModel<T> dialogViewModel = null)
            : base(commonParams, owner, dialogViewModel)
        {
            _uploadableComponent = new FilesComponent<T, F>(this);

            PropertyChanged += OnPropertyChanged;
        }

        public override void Cleanup()
        {
            PropertyChanged -= OnPropertyChanged;
            base.Cleanup();
        }

        public IDragAndDropable DragAndDropComponent => _uploadableComponent;

        public virtual ICommand FilesCmd => _uploadableComponent.FilesCmd;

        public virtual void OnDrop(object sender, DragEventArgs e)
        {
            _uploadableComponent.OnDrop(sender, e);
        }

        private void OnHasFilesChangedMessage(HasFilesChangedMessage<T> m)
        {
            ModelObject.HasFiles = m.UpdatedHasFiles;
        }

        private void OnPropertyChanged(
            object sender, PropertyChangedEventArgs e)
        {
            if (HasFocus && e.PropertyName == nameof(IsItemSelected))
            {
                MessengerInstance.Unregister<HasFilesChangedMessage<T>>(this);
                if (IsItemSelected)
                {
                    MessengerInstance.Register<HasFilesChangedMessage<T>>(
                        this, OnHasFilesChangedMessage);
                }
            }
        }

    }
}
