namespace savaged.mvvm.Core.Interfaces
{
    public interface IModelWithUploads : IObservableModel
    {
        bool HasFiles { get; set; }
    }
}
