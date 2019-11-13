using savaged.mvvm.Navigation;
using System.Collections.ObjectModel;

namespace savaged.mvvm.Core.Interfaces
{
    public interface ILogDialogViewModel<TLoggedModel>
        : IViewManager, INavigableDialogViewModel
        where TLoggedModel : IObservableModel
    {
        ObservableCollection<IModelLog> Index { get; }

        void Seed(TLoggedModel loggedModel);
        
    }
}