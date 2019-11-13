using savaged.mvvm.Core.Interfaces;
using savaged.mvvm.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace savaged.mvvm.ViewModels.Core.Utils
{
    public class FileHelperService : IFileHelperService
    {
        private const string _defaultFileFilter =
            "Adobe (*.pdf)|*.pdf|Older Word docs (*.doc)|*.doc|" +
            "Word docs (*.docx)|*.docx|Older Excel (*.xls)|*.xls|Excel (*.xlsx)|*.xlsx|" +
            "Archives(*.zip) | *.zip|Joint Photo Experts Group image (*.jpeg)|*.jpeg|" +
            "Portable Network Graphics image (*.png)|*.png|Bitmap image (*.bmp)|*.bmp|" +
            "Tagged image (*.tiff)|*.tiff";

        private readonly ISystemDialogService _systemDialogService;

        public FileHelperService(ISystemDialogService systemDialogService)
        {
            _systemDialogService = systemDialogService;
        }

        public void Cleanup(string userFileLocation)
        {
            if (userFileLocation.Contains(Path.GetTempPath()))
            {
                File.Delete(userFileLocation);
            }
        }

        public string GetUserFileLocation(
            string userFileLocation,
            string fileFilter = null)
        {
            var isValidPath = true;
            string path = null;
            if (userFileLocation != null)
            {
                try
                {
                    path = Path.GetFullPath(userFileLocation);
                }
                catch
                {
                    isValidPath = false;
                }
            }
            if (string.IsNullOrEmpty(fileFilter))
            {
                fileFilter = _defaultFileFilter;
            }

            string filename = null;
            var title = "Upload File Location";
            var initialDirectory = Environment.GetFolderPath(
                Environment.SpecialFolder.MyDocuments);
            var filter = fileFilter;
            if (isValidPath)
            {
                filename = path.Split('\\').Last();
                initialDirectory = path.Substring(0, path.LastIndexOf('\\'));
                var extension = path.Split('.').Last();
                fileFilter = $"Selected (*.{extension})|*.{extension}";
            }

            userFileLocation = _systemDialogService.ShowOpenFileDialog(
                title, initialDirectory, fileFilter, filename);

            return userFileLocation;
        }


        public async Task<string> DownloadFile<T>(
            object caller,
            T selected,
            IFileService fileService,
            IDictionary<string, object> args = null)
            where T : IFileModel
        {
            var fileInfo = await fileService.DownloadAsync(
                AuthUser.Current, selected);
            var filePath = fileInfo.FullName;
            return filePath;
        }

        public async Task<string> DownloadFile<T>(
            object caller,
            int selectedId,
            IFileService fileService,
            IDictionary<string, object> args = null)
            where T : IFileModel
        {
            var fileInfo = await fileService.DownloadAsync<T>(
                AuthUser.Current, selectedId);
            var filePath = fileInfo.FullName;
            return filePath;
        }


        public async Task<bool> DownloadAndShowFile<T>(
            object caller,
            T selected,
            IFileService fileService,
            IDictionary<string, object> args = null)
            where T : IFileModel
        {
            var filePath = await DownloadFile(
                caller, selected, fileService);

            if (filePath != string.Empty)
            {
                Process.Start(filePath);
            }
            return true;
        }

        public async Task<bool> DownloadAndShowFile<T>(
            object caller,
            int selectedId,
            IFileService fileService,
            IDictionary<string, object> args = null)
            where T : IFileModel
        {
            var filePath = await DownloadFile<T>(
                caller, selectedId, fileService);

            if (filePath != string.Empty)
            {
                Process.Start(filePath);
            }
            return true;
        }

    }
}
