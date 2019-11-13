using System.Collections.Generic;

namespace savaged.mvvm.Core.Interfaces
{
    public interface IDualModeViewModel<T> : IViewModel
        where T : IObservableModel, new()
    {
        IIndexViewModel<T> IndexViewModel { get; }
        ISelectedItemViewModel<T> SelectedItemViewModel { get; set; }

        void Seed(IObservableModel parent, IEnumerable<T> index);

        void ResetModelObjectToHeader();

        void Clear();

        bool OverrideSelectFirst { get; set; }
    }
}