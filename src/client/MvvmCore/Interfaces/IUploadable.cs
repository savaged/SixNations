using System.Windows.Input;

namespace savaged.mvvm.Core.Interfaces
{
    public interface IUploadable : IDragAndDropable
    {
        ICommand FilesCmd { get; }
    }
}
