namespace savaged.mvvm.Core.Interfaces
{
    public interface ISelectedItemEventArgs<T>
        where T : IObservableModel
    {
        T SelectedItem { get; }
    }
}