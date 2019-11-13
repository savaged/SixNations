using savaged.mvvm.Navigation;

namespace savaged.mvvm.Core.Interfaces
{
    public interface IDualModeDialogViewModel<T> 
        : IDualModeViewModel<T>, INavigableDialogViewModel
        where T : IObservableModel, new()
    {
        bool FormVisible { get; set; }
        bool IndexVisible { get; }
    }
}