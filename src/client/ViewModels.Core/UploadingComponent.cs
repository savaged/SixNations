using savaged.mvvm.Core.Interfaces;
using savaged.mvvm.Data;
using savaged.mvvm.ViewModels.Core.Utils;
using savaged.mvvm.ViewModels.Core.Utils.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace savaged.mvvm.ViewModels.Core
{
    public class UploadingComponent<T, F>
        : ViewModelBase, IUploading<T, F>
        where T : IModelWithUploads, new()
        where F : IFileModel, new()
    {
        private readonly IUploading<T, F> _owner;
        private readonly IModelService _modelService;
        
        public UploadingComponent(
            IUploading<T, F> owner,
            IFileHelperService fileHelper,
            IModelService modelService)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));

            FileHelper = fileHelper ??
                throw new ArgumentNullException(nameof(fileHelper));

            _modelService = modelService ?? 
                throw new ArgumentNullException(nameof(modelService));

            GetUserFileLocationCmd = new RelayCommand(OnFormUserFileLocation);
        }

        public bool IsDirty
        {
            get => _owner.IsDirty;
            set => _owner.IsDirty = value;
        }

        public IFileHelperService FileHelper { get; }

        public string UserFileLocation
        {
            get => _owner.UserFileLocation;
            set
            {
                _owner.UserFileLocation = value;
                if (!string.IsNullOrEmpty(value))
                {
                    IsDirty = true;
                }
            }
        }

        public F FileModel
        {
            get => _owner.FileModel;
            set => _owner.FileModel = value;
        }

        public T ModelWithUploads => _owner.ModelWithUploads;

        public ICommand GetUserFileLocationCmd { get; }

        public async Task CreateFileModelAsync()
        {
            MessengerInstance.Send(new BusyMessage(true, this));
            try
            {
                FileModel = new F();
                var created = await _modelService.CreateAsync<F>(
                    AuthUser.Current, ModelWithUploads);
                FileModel.FileName = created?.FileName;
                FileModel.IsUnlocked = true;
            }
            finally
            {
                MessengerInstance.Send(new BusyMessage(false, this));
            }
        }

        public async Task UploadAsync()
        {
            if (string.IsNullOrEmpty(UserFileLocation))
            {
                return;
            }
            if (string.IsNullOrEmpty(FileModel?.FileName))
            {
                await CreateFileModelAsync();
            }
            FileModel.UploadedFile = UserFileLocation;
            FileModel.Extension = UserFileLocationInfo.Extension.ToLower();

            var saved = await _owner.StoreFileModelAsync();

            if (saved == null)
            {
                throw new InvalidOperationException(
                    "The persist file operation failed " +
                    "to return an updated file-upload!");
            }
            if (saved.IsNew)
            {
                throw new InvalidOperationException(
                    "The persist file operation returned a " +
                    "file-upload, but with no ID mapped!");
            }
            if (ModelWithUploads != null)
            {
                ModelWithUploads.HasFiles = true;
            }
            Cleanup();
        }

        public async Task<F> StoreFileModelAsync()
        {
            if (ModelWithUploads == null)
            {
                throw new InvalidOperationException(
                    $"The {nameof(ModelWithUploads)} should be set by here!");
            }
            var saved = await _modelService.StoreAsync(
                AuthUser.Current, FileModel, ModelWithUploads);
            return saved;
        }

        public override void Cleanup()
        {
            FileHelper.Cleanup(UserFileLocation);
            UserFileLocation = string.Empty;
            if (FileModel != null)
            {
                FileModel.FileName =
                    FileModel.Extension =
                    FileModel.UploadedFile = string.Empty;
            }
            base.Cleanup();
        }

        public void OnDrop(object sender, DragEventArgs e)
        {
            MessengerInstance.Send(new BusyMessage(true, this));
            try
            {
                UserFileLocation = 
                    DragDropHelper.DraggedItemToUserFileLocation(e);
            }
            finally
            {
                MessengerInstance.Send(new BusyMessage(false, this));
            }
        }

        public FileInfo UserFileLocationInfo => new FileInfo(UserFileLocation);

        private void OnFormUserFileLocation()
        {
            var value = FileHelper.GetUserFileLocation(UserFileLocation);
            UserFileLocation = value;
        }

    }
}
