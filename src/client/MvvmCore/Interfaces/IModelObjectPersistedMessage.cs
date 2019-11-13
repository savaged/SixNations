namespace savaged.mvvm.Core.Interfaces
{
    public interface IModelObjectPersistedMessage<T> 
        where T : IObservableModel, new()
    {
        bool IsAddition { get; }
        bool IsDeletion { get; }
        bool ModelObjectUpdateImpactsRelations { get; }
        bool ModelObjectUpdateWithoutIndexReload { get; }
        T Old { get; }
        T Updated { get; }
        IObservableModel Parent { get; }
    }
}