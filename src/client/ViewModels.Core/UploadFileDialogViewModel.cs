using savaged.mvvm.Core.Interfaces;
using savaged.mvvm.Data;
using savaged.mvvm.Navigation;
using savaged.mvvm.ViewModels.Core.Utils.Messages;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace savaged.mvvm.ViewModels.Core
{
    public class UploadFileDialogViewModel<R, F>
        : DualModeDialogViewModel<F>,
        IUploadFileDialogViewModel<R, F>
        where R : IModelWithUploads, new()
        where F : IFileModel, new()
    {
        private readonly IUploading<R, F> _uploadingComponent;
        private string _userFileLocation;
        private string _downloadLocation;

        public UploadFileDialogViewModel(
            IViewModelCommonParams commonParams,
            IOwnedFocusable owner,
            bool forceFormVisible = false)
            : base(commonParams, owner)
        {
            CtorArgs = new object[]
            {
                commonParams, owner, forceFormVisible
            };

            _uploadingComponent = new UploadingComponent<R, F>(
                this, CommonParams?.FileHelper, ModelService);
            _userFileLocation = string.Empty;

            FormVisible = forceFormVisible;

            ShowUploadedFileCmd = new RelayCommand(
                OnShowUploadedFile, () => CanShowUploadedFiles);
            DownloadUploadedFileCmd = new RelayCommand(
                OnDownloadUploadedFile, () => CanShowUploadedFiles);
        }

        public override void Cleanup()
        {
            _uploadingComponent.Cleanup();
            base.Cleanup();
        }

        public void Seed(R modelWithUploads)
        {
            base.Seed(modelWithUploads, null);
            FormVisible = !HasFiles;
        }

        public async override Task<bool> LoadAsync()
        {
            var result = false;
            MessengerInstance.Send(new BusyMessage(true, this));
            try
            {
                if (ModelWithUploads == null)
                {
                    throw new InvalidOperationException(
                        $"The {nameof(ModelWithUploads)} should be set by here!");
                }
                result = await base.LoadAsync();
            }
            finally
            {
                MessengerInstance.Send(new BusyMessage(false, this));
            }
            return result;
        }

        public IFileHelperService FileHelper => _uploadingComponent.FileHelper;

        public ICommand ShowUploadedFileCmd { get; }

        public ICommand DownloadUploadedFileCmd { get; }

        public ICommand GetUserFileLocationCmd => 
            _uploadingComponent.GetUserFileLocationCmd;

        public virtual bool CanShowUploadedFiles => CanExecute && HasFiles;

        public override bool CanSave => CanExecute && 
            !string.IsNullOrEmpty(UserFileLocation);

        public string DownloadLocation
        {
            get => _downloadLocation;
            set => Set(ref _downloadLocation, value);
        }

        public string UserFileLocation
        {
            get => _userFileLocation;
            set => Set(ref _userFileLocation, value);
        }

        public FileInfo UserFileLocationInfo =>
           _uploadingComponent.UserFileLocationInfo;

        public R ModelWithUploads => (R)Parent;

        public F FileModel
        {
            get => ModelObject;
            set
            {
                ModelObject = value;
                RaisePropertyChanged();
            }
        }

        public void OnDrop(object sender, DragEventArgs e)
        {
            FormVisible = true;
            _uploadingComponent.OnDrop(sender, e);
        }

        public virtual async Task CreateFileModelAsync()
        {
            await _uploadingComponent.CreateFileModelAsync();
        }

        public virtual async Task UploadAsync()
        {
            await _uploadingComponent.UploadAsync();
        }

        public virtual async Task<F> StoreFileModelAsync()
        {
            return await _uploadingComponent.StoreFileModelAsync();
        }

        public override bool ConfirmDelete(
            string additionalMsg = null, bool isArchiveOnly = false)
        {
            return base.ConfirmDelete(
                "WHEN DELETED THIS FILE CANNOT BE RETRIEVED; " +
                "IT IS GONE FOR GOOD!", isArchiveOnly);
        }

        protected override async Task Add()
        {
            await CreateFileModelAsync();
            FormVisible = true;
        }

        protected override async Task<bool> EditAsync()
        {
            if (FileModel == null)
            {
                throw new InvalidOperationException(
                    "The model object should be set by here!");
            }
            FileModel.IsUnlocked = true;
            return await AlwaysTrue();
        }

        protected override async Task<bool> FormSave()
        {
            var result = false;
            MessengerInstance.Send(new BusyMessage(true, this));
            try
            {
                var oldHasFilesValue = HasFiles;

                await UploadAsync();
                FormVisible = false;
                await LoadAsync();

                RaiseHasFilesChange(oldHasFilesValue, true);

                result = true;
            }
            catch (ApiValidationException ex)
            {
                ReactToException(this, ex);
                result = false;
            }
            finally
            {
                MessengerInstance.Send(new BusyMessage(false, this));
            }
            await ReactToSuccessfulSave();
            return result;
        }

        protected async override Task<bool> FormDelete()
        {
            var oldHasFilesValue = HasFiles;

            var result = false;

            MessengerInstance.Send(new BusyMessage(true, this));
            try
            {
                result = await QuietDeleteAsync();
            }
            catch (DesktopException ex)
            {
                ReactToException(this, ex);
            }
            finally
            {
                MessengerInstance.Send(new BusyMessage(false, this));
            }
            FormVisible = false;

            if (result)
            {
                var newHasFilesValue = await GetUpdatedHasFilesAsync();
                RaiseHasFilesChange(oldHasFilesValue, newHasFilesValue);

                IsDirty = !result;

                if (newHasFilesValue)
                {
                    await LoadAsync();
                }
                else
                {
                    CloseView(true);
                }
            }
            return result;
        }

        protected virtual bool HasFiles => ModelWithUploads?.HasFiles == true;

        protected virtual async Task<bool> GetUpdatedHasFilesAsync()
        {
            var value = await ModelService.ShowFieldValueAsync<R, bool>(
                AuthUser.Current,
                ModelWithUploads,
                nameof(ModelWithUploads.HasFiles));
            return value;
        }

        protected virtual void RaiseHasFilesChange(bool oldValue, bool newValue)
        {
            if (oldValue != newValue)
            {
                ModelWithUploads.HasFiles = newValue;
                MessengerInstance.Send(new HasFilesChangedMessage<R>(
                    this, ModelWithUploads));
            }
        }

        private async void OnShowUploadedFile()
        {
            MessengerInstance.Send(new BusyMessage(true, this));
            try
            {
                await FileHelper.DownloadAndShowFile(
                    this,
                    FileModel,
                    CommonParams.FileService);
            }
            catch (DesktopException ex)
            {
                ReactToException(this, ex);
            }
            finally
            {
                MessengerInstance.Send(new BusyMessage(false, this));
            }
        }

        private async void OnDownloadUploadedFile()
        {
            DownloadLocation = string.Empty;
            MessengerInstance.Send(new BusyMessage(true, this));
            try
            {
                DownloadLocation = await FileHelper.DownloadFile(
                    this,
                    FileModel,
                    CommonParams.FileService);

                if (File.Exists(DownloadLocation))
                {
                    Process.Start(
                        "explorer.exe", $"/select, {DownloadLocation}");
                }
            }
            catch (DesktopException ex)
            {
                ReactToException(this, ex);
            }
            finally
            {
                MessengerInstance.Send(new BusyMessage(false, this));
            }
        }

    }
}
