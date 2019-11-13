namespace savaged.mvvm.Core.Interfaces
{
    public interface IFileModel<R> : IFileModel 
        where R : IModelWithUploads, new() {}

    public interface IFileModel : IObservableModel
    {
        string FileName { get; set; }
        string Extension { get; set; }

        string UploadedFile { get; set; }
    }
}