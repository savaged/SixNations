using savaged.mvvm.Core.Interfaces;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace savaged.mvvm.ViewModels.Core.Utils
{
    public class EnumerableToObservableCollectionConverter<T> 
        where T : IObservableModel
    {
        private ObservableCollection<T> _collection;

        public EnumerableToObservableCollectionConverter(
            ObservableCollection<T> collectionToSet)
        {
            _collection = collectionToSet ?? 
                throw new ArgumentException(nameof(collectionToSet),
                 $"The ObservableCollection<{typeof(T).Name}> to be " +
                 "set should be initiated so that Xaml can bind to it. " +
                 "Typically this can be done in the ViewModel ctor.");
        }

        public void UpdateCollection(IEnumerable<T> index)
        {
            ClearCollection();
            AddCollection(index);
        }

        public void ClearCollection()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => _collection.Clear());
        }

        public void AddCollection(IEnumerable<T> index)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                if (index != null && index.Count() > 0)
                {
                    foreach (var item in index)
                    {
                        _collection.Add(item);
                    }
                }
            });
        }

        public void UpdateCollection(
            T old, T updated, bool isDeletion = false, bool isAddition = false)
        {
            Validate(old, updated);

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                var item = _collection.FirstOrDefault(i => i.Id == old.Id);

                if (_collection.Contains(item))
                {
                    var indexOfSelectedItem = _collection.IndexOf(item);

                    if (isDeletion)
                    {
                        UpdateCollectionOnDelete(item);
                    }
                    else if (isAddition)
                    {
                        UpdateCollectionOnAdd(item);
                    }
                    else
                    {
                        _collection.Remove(item);
                        _collection.Insert(indexOfSelectedItem, updated);
                    }
                }
                else
                {
                    _collection.Remove(item);
                    _collection.Insert(0, updated);
                }
            });
        }

        public void UpdateCollectionOnDelete(T old)
        {
            Validate(old);

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                var item = _collection.FirstOrDefault(i => i.Id == old.Id);

                if (_collection.Contains(item))
                {
                    _collection.Remove(item);
                }
            });
        }

        public void UpdateCollectionOnAdd(T added)
        {
            Validate(added);

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                _collection.Insert(0, added);
            });
        }

        private void Validate(T old, T updated)
        {
            Validate(old);

            if (updated == null)
            {
                throw new ArgumentNullException(nameof(updated));
            }
        }

        private void Validate(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
        }

    }
}
