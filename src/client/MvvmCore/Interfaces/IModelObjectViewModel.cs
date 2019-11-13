namespace savaged.mvvm.Core.Interfaces
{
    public interface IModelObjectViewModel<T> : IOwnedFocusableViewModel
        where T : IObservableModel, new()
    {
        IModelObjectDialogViewModel<T> DialogViewModel { get; set; }
    }
}