using savaged.mvvm.Core.Interfaces;
using System;

namespace savaged.mvvm.ViewModels.Core.Utils
{
    public class SelectedFirstEventArgs<T> : EventArgs, ISelectedItemEventArgs<T> 
        where T : IObservableModel
    {
        public SelectedFirstEventArgs(T selectedItem)
        {
            SelectedItem = selectedItem;
        }

        public T SelectedItem { get; }

    }
}
