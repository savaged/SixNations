using savaged.mvvm.Navigation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace savaged.mvvm.Core.Interfaces
{
    public interface ILoggedViewModel<T> : IOwnedFocusable
        where T : IObservableModel
    {
        void InitialiseLogDialogViewModel(T loggedModel);

        Task<IEnumerable<IModelLog>> LoadModelLogIndexAsync();
    }
}