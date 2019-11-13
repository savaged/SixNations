using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace savaged.mvvm.Data
{
    public interface IFileService
    {
        Task<FileInfo> DownloadAsync<T>(
            IAuthUser user, 
            T model,
            IDictionary<string, object> args = null)
            where T : IDataModel;

        Task<FileInfo> DownloadAsync<T>(
            IAuthUser user,
            int modelId,
            IDictionary<string, object> args = null)
            where T : IDataModel;

        Task UploadAsync<T>(
            IAuthUser user,
            Type modelWithFilesType,
            T parentToModelWithFiles,
            FileInfo file)
            where T : IDataModel;
    }
}