using savaged.mvvm.Navigation;

namespace savaged.mvvm.Core.Interfaces
{
    public interface IOwnedFocusableViewModel : IViewModel, IOwnedFocusable
    {
        void Clear();

        IObservableModel Parent { get; set; }
    }
}
