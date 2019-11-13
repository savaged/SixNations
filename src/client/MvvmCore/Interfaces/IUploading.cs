using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace savaged.mvvm.Core.Interfaces
{
    public interface IUploading<T, F> : IDragAndDropable
        where T : IModelWithUploads, new()
        where F : IFileModel, new()
    {
        ICommand GetUserFileLocationCmd { get; }

        IFileHelperService FileHelper { get; }

        string UserFileLocation { get; set; }

        FileInfo UserFileLocationInfo { get; }

        F FileModel { get; set; }

        T ModelWithUploads { get; }

        Task CreateFileModelAsync();

        Task UploadAsync();

        Task<F> StoreFileModelAsync();

        void Cleanup();

        bool IsDirty { get; set; }

    }
}