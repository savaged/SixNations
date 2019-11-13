using savaged.mvvm.Core.Interfaces;
using Microsoft.Win32;

namespace savaged.mvvm.Win32DialogService
{
    public class SystemDialogService : ISystemDialogService
    {
        private readonly OpenFileDialog _ofd;

        public SystemDialogService()
        {
            _ofd = new OpenFileDialog();
        }

        public string ShowOpenFileDialog(
            string title = null,
            string initialDirectory = null,
            string fileFilter = null,
            string filename = null)
        {
            _ofd.Reset();

            if (title != null)
            {
                _ofd.Title = title;
            }
            if (initialDirectory != null)
            {
                _ofd.InitialDirectory = initialDirectory;
            }
            if (fileFilter != null)
            {
                _ofd.Filter = fileFilter;
            }
            if (filename != null)
            {
                _ofd.FileName = filename;
            }

            var result = _ofd.ShowDialog();

            if (result == true)
            {
                filename = _ofd.FileName;
            }
            else
            {
                filename = string.Empty;
            }
            return filename;
        }
    }
}
