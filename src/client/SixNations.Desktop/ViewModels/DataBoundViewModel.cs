// Pre Standard .Net (see http://www.mvvmlight.net/std10) using CommonServiceLocator;
using System;
using log4net;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using System.Windows.Input;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using SixNations.Desktop.Helpers;
using SixNations.Desktop.Interfaces;
using SixNations.Data.Models;
using SixNations.API.Interfaces;
using Savaged.BusyStateManager;
using SixNations.Desktop.Services;

namespace SixNations.Desktop.ViewModels
{
    public abstract class DataBoundViewModel<T> : ViewModelBase, IAsyncViewModel
        where T : IHttpDataServiceModel, new()
    {
        private static readonly ILog Log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected readonly IDataService<T> DataService;
        protected readonly IActionConfirmationService ActionConfirmation;
        private readonly bool _withRefreshPolling;
        private T _selectedItem;
        private bool _canSelectItem;

        public DataBoundViewModel(
            IDataService<T> dataService, 
            IActionConfirmationService actionConfirmation,
            bool withRefreshPolling = false)
        {
            DataService = dataService;
            ActionConfirmation = actionConfirmation;
            
            _withRefreshPolling = withRefreshPolling;
            PollingService.Instance.IntervalElapsed += OnPollingIntervalElapsed;

            Index = new ObservableCollection<T>();

            NewCmd = new RelayCommand(OnNew, () => CanExecuteNew);
            EditCmd = new RelayCommand(OnEdit, () => CanExecuteEdit);
            DeleteCmd = new RelayCommand(OnDelete, () => CanExecuteSelectedItemChange);
            SaveCmd = new RelayCommand(OnSave, () => CanExecuteSave);
            CancelCmd = new RelayCommand(OnCancel, () => CanExecuteCancel);
        }

        public override void Cleanup()
        {
            PollingService.Instance.IntervalElapsed -= OnPollingIntervalElapsed;
            base.Cleanup();
        }

        public async virtual Task LoadAsync()
        {
            MessengerInstance.Send(new BusyMessage(true, this));

            if (_withRefreshPolling)
            {
                PollingService.Instance.Start();
            }
            try
            {
                await LoadIndexAsync();

                CanSelectItem = true;
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Unexpexted exception loading! {0}", ex);
                FeedbackActions.ReactToException(ex);
            }
            finally
            {
                MessengerInstance.Send(new BusyMessage(false, this));
            }
        }

        protected virtual async Task LoadIndexAsync()
        {
            IEnumerable<T> data = null;
            if (User.Current.IsLoggedIn)
            {
                data = await GetIndexAsync();
            }
            if (data != null)
            {
                Index.Clear();
                foreach (var item in data)
                {
                    Index.Add(item);
                }
            }
            else
            {
                Index.Clear();
            }
        }

        private async Task<IEnumerable<T>> GetIndexAsync()
        {
            IEnumerable<T> data = null;
            try
            {
                data = await DataService.GetModelDataAsync(
                    User.Current.AuthToken, FeedbackActions.ReactToException);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
            return data;
        }

        public ObservableCollection<T> Index { get; }

        public bool IsSelectedItemEditable => SelectedItem != null &&
            SelectedItem.IsLockedForEditing;

        public bool CanSelectItem
        {
            get => _canSelectItem;
            set => Set(ref _canSelectItem, value);
        }

        public T SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (value == null)
                {
                    Set(ref _selectedItem, value);
                }
                else if (CanSelectItem)
                {
                    Set(ref _selectedItem, value);
                }
                RaisePropertyChanged(nameof(IsSelectedItemEditable));
            }
        }

        public ICommand NewCmd { get; }

        public ICommand EditCmd { get; }

        public ICommand DeleteCmd { get; }

        public ICommand SaveCmd { get; }

        public ICommand CancelCmd { get; }

        // TODO: Add permissions check on current user
        public bool CanExecute
        {
            get
            {
                var value = false;
                var serviceLocator = SimpleIoc.Default;
                var mainVM = serviceLocator.GetInstance<MainViewModel>();
                var busyMngr = mainVM.BusyStateManager;
                value = !busyMngr.IsBusy;
                return value;
            }
        }

        // TODO: Add permissions check on current user
        public bool CanExecuteNew => CanExecute && CanSelectItem;

        // TODO: Add permissions check on current user
        public bool CanExecuteEdit => CanExecute && IsSelectedItemEditable;

        // TODO: Add permissions check on current user
        public bool CanExecuteSelectedItemChange => CanExecute &&
            !IsSelectedItemEditable;

        public bool CanExecuteCancel => IsSelectedItemEditable;

        public bool CanExecuteSave => CanExecute && IsSelectedItemEditable &&
            SelectedItem.IsDirty;

        private async void OnNew()
        {
            MessengerInstance.Send(new BusyMessage(true, this));
            try
            {
                SelectedItem = await DataService.CreateModelAsync(
                    User.Current.AuthToken, FeedbackActions.ReactToException);
                RaisePropertyChanged(nameof(IsSelectedItemEditable));
                CanSelectItem = false;
            }
            catch (Exception ex)
            {
                Log.Error($"Unexpected Error Creating! {ex}");
                throw;
            }
            finally
            {
                MessengerInstance.Send(new BusyMessage(false, this));
            }
        }

        private async void OnEdit()
        {
            MessengerInstance.Send(new BusyMessage(true, this));
            try
            {
                SelectedItem = await DataService.EditModelAsync(
                    User.Current.AuthToken, FeedbackActions.ReactToException, SelectedItem);
                RaisePropertyChanged(nameof(IsSelectedItemEditable));
                CanSelectItem = false;
            }
            catch (Exception ex)
            {
                Log.Error($"Unexpected Error Saving! {ex}");
                throw;
            }
            finally
            {
                MessengerInstance.Send(new BusyMessage(false, this));
            }
        }

        private async void OnDelete()
        {
            var confirmed = ActionConfirmation.Confirm(ActionConfirmations.Delete);
            if (confirmed)
            {
                MessengerInstance.Send(new BusyMessage(true, this));
                bool result;
                try
                {
                    result = await DataService.DeleteModelAsync(
                        User.Current.AuthToken, FeedbackActions.ReactToException, SelectedItem);
                    RaisePropertyChanged(nameof(IsSelectedItemEditable));
                    if (result)
                    {
                        await LoadIndexAsync();

                        SelectedItem = Index.First();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"Unexpected Error Deleting! {ex}");
                    throw;
                }
                finally
                {
                    MessengerInstance.Send(new BusyMessage(false, this));
                }
            }
        }

        private async void OnSave()
        {
            MessengerInstance.Send(new BusyMessage(true, this));
            try
            {
                T updated;
                if (SelectedItem.IsNew)
                {
                    updated = await DataService.StoreModelAsync(
                        User.Current.AuthToken, FeedbackActions.ReactToException, SelectedItem);
                }
                else
                {
                    updated = await DataService.UpdateModelAsync(
                        User.Current.AuthToken, FeedbackActions.ReactToException, SelectedItem);
                }
                RaisePropertyChanged(nameof(IsSelectedItemEditable));
                CanSelectItem = true;
                if (updated != null)
                {
                    await LoadIndexAsync();
                    SelectedItem = Index.FirstOrDefault(r => r.Id == updated.Id);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Unexpected Error Saving! {ex}");
                throw;
            }
            finally
            {
                MessengerInstance.Send(new BusyMessage(false, this));
            }
        }

        private async void OnCancel()
        {
            var confirmed = !SelectedItem.IsDirty;
            if (!confirmed)
            {
                confirmed = ActionConfirmation.Confirm(ActionConfirmations.Cancel);
            }
            if (confirmed)
            {
                MessengerInstance.Send(new BusyMessage(true, this));
                await LoadIndexAsync();
                CanSelectItem = true;
                SelectedItem = Index.FirstOrDefault();
                MessengerInstance.Send(new BusyMessage(false, this));
            }
        }

        private void OnPollingIntervalElapsed()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(async () =>
            {
                await LoadAsync();
            });
        }
    }
}
