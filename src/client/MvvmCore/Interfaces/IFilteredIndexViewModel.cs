namespace savaged.mvvm.Core.Interfaces
{
    public interface IFilteredIndexViewModel<T> : IIndexViewModel<T>
        where T : IObservableModel, new()
    {
        IIndexFilters IndexFilters { get; }
    }
}