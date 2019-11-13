namespace savaged.mvvm.Core.Interfaces
{
    public interface ISelectedItemWithUploadsViewModel<T, F>
        : IDragDropSelectedItemViewModel<T>
        where T : IModelWithUploads, new()
        where F : IFileModel, new()
    {
    }
}