namespace savaged.mvvm.Core.Interfaces
{
    public interface IDragDropSelectedItemViewModel<T>
        : ISelectedItemViewModel<T>, IUploadableViewModel
        where T : IObservableModel, new()
    {
        IDragAndDropable DragAndDropComponent { get; }
    }
}
