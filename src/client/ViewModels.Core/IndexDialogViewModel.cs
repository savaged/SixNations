using savaged.mvvm.Core.Interfaces;
using savaged.mvvm.Navigation;

namespace savaged.mvvm.ViewModels.Core
{
    public abstract class IndexDialogViewModel<T> 
        : IndexViewModel<T>, INavigableDialogViewModel
        where T : IObservableModel, new()
    {
        public IndexDialogViewModel(
            IViewModelCommonParams commonParams)
            : base(commonParams)
        {
            HasFocus = true;
        }
    }
}
