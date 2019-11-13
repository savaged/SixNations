using savaged.mvvm.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace savaged.mvvm.Core.Interfaces
{
    public interface IIndexViewModel<T> : IModelObjectViewModel<T>, IViewManager
        where T : IObservableModel, new()
    {
        void Seed(IObservableModel parent, IEnumerable<T> index = null);

        void Seed(IEnumerable<T> index, IObservableModel parent = null);

        bool DisableBusyMessages { get; }

        bool OverrideSelectFirst { get; set; }

        ObservableCollection<T> Index { get; }

        IModelService ModelService { get; }

        ICommand AddCmd { get; set; }

        bool CanAdd { get; }

        Task<bool> UpdateIndexAsync(IModelObjectPersistedMessage<T> m);

        event EventHandler<ISelectedItemEventArgs<T>> SelectedFirst;
    }
}