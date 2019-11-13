using System.Collections.Generic;
using System.Threading.Tasks;
using savaged.mvvm.Data;

namespace savaged.mvvm.Core.Interfaces
{
    public interface IFileHelperService
    {
        void Cleanup(string userFileLocation);
        Task<bool> DownloadAndShowFile<T>(object caller, int selectedId, IFileService fileService, IDictionary<string, object> args = null) where T : IFileModel;
        Task<bool> DownloadAndShowFile<T>(object caller, T selected, IFileService fileService, IDictionary<string, object> args = null) where T : IFileModel;
        Task<string> DownloadFile<T>(object caller, int selectedId, IFileService fileService, IDictionary<string, object> args = null) where T : IFileModel;
        Task<string> DownloadFile<T>(object caller, T selected, IFileService fileService, IDictionary<string, object> args = null) where T : IFileModel;
        string GetUserFileLocation(string userFileLocation, string fileFilter = null);
    }
}