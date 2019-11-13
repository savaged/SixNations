using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace savaged.mvvm.Data
{
    public class FileService : IFileService
    {
        private readonly IDataServiceGateway _dataServiceGateway;
        private readonly string _downloadLocation;
        private readonly ApiSettings _apiSettings;

        public FileService(
            string baseUrl,
            ApiSettings apiSettings,
            string downloadLocation)
        {
            _dataServiceGateway = new DataServiceGateway(
                baseUrl, apiSettings);

            _downloadLocation = downloadLocation;

            _apiSettings = apiSettings ??
                throw new ArgumentNullException(nameof(apiSettings));

            if (string.IsNullOrEmpty(_apiSettings.UploadedFileKey))
            {
                throw new ArgumentNullException(
                    nameof(_apiSettings.UploadedFileKey), 
                    $"The {nameof(ApiSettings)} must include a value for " +
                    nameof(_apiSettings.UploadedFileKey));
            }
        }

        public async Task<FileInfo> DownloadAsync<T>(
            IAuthUser user, 
            T model,
            IDictionary<string, object> args = null)
            where T : IDataModel
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var uri = new UriActionBuilder().BuildShow(model, args);
            var fileInfo = await DownloadAsync(user, uri);
            return fileInfo;
        }

        public async Task<FileInfo> DownloadAsync<T>(
            IAuthUser user,
            int modelId,
            IDictionary<string, object> args = null)
            where T : IDataModel
        {
            var uri = 
                new UriActionBuilder().BuildShow(typeof(T).Name, modelId, args);
            var fileInfo = await DownloadAsync(user, uri);
            return fileInfo;
        }

        private async Task<FileInfo> DownloadAsync(IAuthUser user, string uri)
        {
            var response = await
                _dataServiceGateway.GetFileAsync(user, uri);
            var fileInfo = Download(uri, response);
            return fileInfo;
        }


        public async Task UploadAsync<T>(
            IAuthUser user,
            Type modelWithFilesType,
            T parentToModelWithFiles,
            FileInfo file)
            where T : IDataModel
        {
            if (modelWithFilesType == null)
            {
                throw new ArgumentNullException(
                    nameof(modelWithFilesType));
            }
            if (parentToModelWithFiles == null)
            {
                throw new ArgumentNullException(
                    nameof(parentToModelWithFiles));
            }
            if (file is null || !file.Exists)
            {
                throw new ArgumentNullException(nameof(file));
            }
            var isFileInUse = IsFileInUse(file);
            if (isFileInUse is null || isFileInUse == true)
            {
                throw new ApiFileInputException(file.FullName,
                    "The file is unavailable because it is open or " +
                    "is being processed by another thread!");
            }

            var uri = new UriActionBuilder().BuildIndex(
                modelWithFilesType, parentToModelWithFiles);

            var data = new Dictionary<string, object>
            {
                { _apiSettings.UploadedFileKey, file }
            };

            await _dataServiceGateway.DataAsync(
                user, uri, HttpMethods.Post, data);
        }

        private FileInfo Download(
            string uri, IResponseFileStream response)
        {
            if (response is null
                || string.IsNullOrEmpty(response.FileName)
                || response.Stream is null
                || response.Stream.Length == 0)
            {
                throw new ArgumentNullException(nameof(response));
            }
            var dir = 
                $"{_downloadLocation}{ConvertWebUriToLocalUri(uri)}\\";
            var path = string.Empty;
            try
            {
                Directory.CreateDirectory(dir);

                path = $"{dir}{response.FileName}";

                using (Stream file = File.Create(path))
                {
                    byte[] buffer = new byte[8 * 1024];
                    int len;
                    while ((len = response.Stream.Read(
                        buffer, 0, buffer.Length)) > 0)
                    {
                        file.Write(buffer, 0, len);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DesktopException(
                    $"Unexpected exception writing '{path}'! " +
                    "The file may already be open or locked.", ex);
            }
            finally
            {
                response.Stream.Close();
            }
            var fileInfo = new FileInfo(path);
            return fileInfo;
        }

        private string ConvertWebUriToLocalUri(string uri)
        {
            uri = uri.Replace("/", "-");

            if (uri.Contains("?"))
            {
                uri = uri.Remove(uri.IndexOf('?'));
            }
            return uri;
        }

        private bool? IsFileInUse(FileInfo file)
        {
            if (file is null || !file.Exists)
            {
                return null;
            }
            FileStream stream = null;
            try
            {
                stream = file.Open(
                    FileMode.Open, 
                    FileAccess.ReadWrite, 
                    FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
            return false;
        }
    }
}
