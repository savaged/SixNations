using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace savaged.mvvm.ViewModels.Core.Utils
{
    public class ListToObservableCollectionConverter<T> 
        where T : INotifyPropertyChanged
    {
        private ObservableCollection<T> _collection;

        public ListToObservableCollectionConverter(
            ObservableCollection<T> collectionToSet)
        {
            _collection = collectionToSet ?? throw new ArgumentException(
                 "The ObservableCollection<" + typeof(T).Name +
                 "> to be set should be initiated so that Xaml can bind to it. " +
                 "Typically this can be done in the ViewModel ctor.");
        }

        public void UpdateCollection(IList<T> list)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                _collection.Clear();
                foreach (var item in list)
                {
                    _collection.Add(item);
                }
            });
        }
    }

    
}
