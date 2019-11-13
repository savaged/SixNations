using savaged.mvvm.Core;
using savaged.mvvm.Core.Interfaces;
using savaged.mvvm.Data;
using savaged.mvvm.Navigation;
using savaged.mvvm.ViewModels.Core.Utils;
using savaged.mvvm.ViewModels.Core.Utils.Messages;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace savaged.mvvm.ViewModels.Core
{
    /// <summary>
    /// As of version 2 this is the main way to present a list of
    /// model objects. In XaML one can add the second main view 
    /// model which will be the 'Selected Item', namely a 
    /// DataModelViewModel.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IndexViewModel<T>
        : BaseModelObjectViewModel<T>, IFilteredIndexViewModel<T>
        where T : IObservableModel, new()
    {
        private readonly EnumerableToObservableCollectionConverter<T>
            _indexConverter;
        private readonly bool _disableBusyMessages;

        /// <summary>
        /// NOTE: The parent object is required because this
        /// class is responsible for creating a ModelObject.
        /// However, this can be late bound through the property.
        /// </summary>
        /// <param name="commonParams"></param>
        /// <param name="owner">ViewModel that has the top level state</param>
        /// <param name="dialogViewModel"></param>
        /// <param name="parent"></param>
        /// <param name="selectItemWhenIndexCountIsOne"></param>
        /// <param name="disableBusyMessages"></param>
        /// <param name="overrideSelectFirst"></param>
        public IndexViewModel(
            IViewModelCommonParams commonParams,
            IOwnedFocusable owner = null,
            IModelObjectDialogViewModel<T> dialogViewModel = null,
            IObservableModel parent = null,
            bool disableBusyMessages = false,
            bool overrideSelectFirst = false)
            : base(commonParams, owner, dialogViewModel)
        {
            Parent = parent.Clone();

            _disableBusyMessages = disableBusyMessages;
            OverrideSelectFirst = overrideSelectFirst;

            Index = new ObservableCollection<T>();
            IndexFilters = new IndexFilters();

            AddCmd = new RelayCommand(OnAdd, () => CanAdd);

            IndexToExcelCmd = new RelayCommand(
                OnIndexToExcel, () => CanExecute);

            _indexConverter =
                new EnumerableToObservableCollectionConverter<T>(Index);
        }

        public virtual void Seed(
            IObservableModel parent, IEnumerable<T> index = null)
        {
            Seed(index, parent);
        }

        public virtual void Seed(
            IEnumerable<T> index, IObservableModel parent = default)
        {
            if (parent != null)
            {
                Parent = parent;
            }
            PopulateIndex(index);
        }

        public override async Task<bool> LoadAsync()
        {
            if (!HasFocus) return false;

            if (!DisableBusyMessages)
            {
                MessengerInstance.Send(new BusyMessage(true, this));
            }
            try
            {
                await LoadIndexAsync();
            }
            catch (DesktopException ex)
            {
                ReactToException(this, ex);
            }
            finally
            {
                if (!DisableBusyMessages)
                {
                    MessengerInstance.Send(new BusyMessage(false, this));
                }
            }
            return true;
        }

        public override void Clear()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() => Index.Clear());
        }

        public virtual async Task<bool> UpdateIndexAsync(
            IModelObjectPersistedMessage<T> m)
        {
            var result = false;

            if (!HasFocus) return false;

            if (m.ModelObjectUpdateImpactsRelations)
            {
                MessengerInstance.Send(new RequestReloadMessage(
                    this, Owner as IOwnedFocusable));
                result = true;
            }
            else
            {
                if (m.ModelObjectUpdateWithoutIndexReload)
                {
                    if (m.IsDeletion)
                    {
                        _indexConverter.UpdateCollectionOnDelete(m.Old);
                    }
                    else if (m.IsAddition)
                    {
                        _indexConverter.UpdateCollectionOnAdd(m.Updated);
                    }
                    else
                    {
                        _indexConverter.UpdateCollection(m.Old, m.Updated);
                    }
                    result = true;
                }
                else
                {
                    result = await LoadAsync();
                }
            }
            return result;
        }

        public virtual bool DisableBusyMessages => _disableBusyMessages;

        public IIndexFilters IndexFilters { get; }

        public ObservableCollection<T> Index { get; }

        public virtual bool CanAdd => CanExecute;

        public ICommand AddCmd { get; set; }

        public ICommand IndexToExcelCmd { get; }

        public bool OverrideSelectFirst { get; set; }

        public event EventHandler<ISelectedItemEventArgs<T>> SelectedFirst =
            delegate { };

        protected virtual async Task LoadIndexAsync()
        {
            IEnumerable<T> index;
            if (IndexFilters?.IsAnySet == true)
            {
                index = await ModelService.IndexAsync<T>(
                    AuthUser.Current, Parent, IndexFilters.Args);
            }
            else
            {
                index = await ModelService.IndexAsync<T>(
                    AuthUser.Current, Parent);
            }
            PopulateIndex(index);
        }

        protected virtual void IndexToExcel()
        {
            var adapter = new IndexToExcelAdapter<T>(Index);
            // Run synchronously
            Task.Run(() => adapter.Adapt());
        }

        private void PopulateIndex(IEnumerable<T> index)
        {
            if (index == null) return;

            _indexConverter.UpdateCollection(index);

            var isEmpty = false;
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                isEmpty = Index.Count < 1;
            });
            if (!OverrideSelectFirst && !isEmpty)
            {
                T firstItem = GetFirstItem();
                if (firstItem?.Equals(default) == false)
                {
                    RaiseSelectedFirst(firstItem);
                }
            }
        }

        private void OnIndexToExcel()
        {
            MessengerInstance.Send(new BusyMessage(true, this));
            try
            {
                IndexToExcel();
            }
            finally
            {
                MessengerInstance.Send(new BusyMessage(false, this));
            }
        }

        protected virtual void OnAdd()
        {
            ShowDialog();
        }

        private T GetFirstItem()
        {
            T value = default;
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                value = Index.FirstOrDefault();
            });
            return value;
        }

        private void RaiseSelectedFirst(T selectedItem)
        {
            SelectedFirst?.Invoke(
                this, new SelectedFirstEventArgs<T>(selectedItem));
        }

    }
}
