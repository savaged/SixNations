using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using log4net;
using SixNations.Desktop.Helpers;
using SixNations.Desktop.Interfaces;
using SixNations.Desktop.Messages;
using SixNations.Desktop.Models;

namespace SixNations.Desktop.ViewModels
{
    // [SuppressMessage("Await.Warning", "CS4014:Await.Warning", Justification = "LoadAsync() runs in the ctor only at design time. [Used a discard instead]")]
    public class RequirementViewModel : ViewModelBase, IAsyncViewModel
    {
        private static readonly ILog Log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IDataService<Requirement> _requirementDataService;
        private readonly IDataService<Lookup> _lookupDataService;
        private Requirement _selectedItem;
        private bool _canSelectItem;
        private Lookup _estimationLookup;
        private Lookup _priorityLookup;
        private Lookup _statusLookup;
        private string _storyFilter;

        public RequirementViewModel(
            IDataService<Requirement> requirementDataService,
            IDataService<Lookup> lookupService)
        {
            _storyFilter = string.Empty;

            _requirementDataService = requirementDataService;
            _lookupDataService = lookupService;

            NewCmd = new RelayCommand(OnNew, () => CanExecuteNew);
            EditCmd = new RelayCommand(OnEdit, () => CanExecuteSelectedItemChange);
            DeleteCmd = new RelayCommand(OnDelete, () => CanExecuteSelectedItemChange);
            SaveCmd = new RelayCommand(OnSave, () => CanExecuteSave);
            CancelCmd = new RelayCommand(OnCancel, () => CanExecuteCancel);

            MessengerInstance.Register<StoryFilterMessage>(this, OnFindStory);

            Index = new ObservableCollection<Requirement>();
        }

        public async Task LoadAsync()
        {
            MessengerInstance.Send(new BusyMessage(true));

            await LoadLookupAsync();

            await LoadIndexAsync();

            CanSelectItem = true;

            SelectedItem = Index.First();

            MessengerInstance.Send(new BusyMessage(false));
        }

        private async Task LoadLookupAsync()
        {
            IEnumerable<Lookup> lookups = null;
            lookups = await _lookupDataService.GetModelDataAsync(
                        User.Current.AuthToken, FeedbackActions.ReactToException);

            EstimationLookup = lookups.First(l => l.Name == "RequirementEstimation");
            PriorityLookup = lookups.First(l => l.Name == "RequirementPriority");
            StatusLookup = lookups.First(l => l.Name == "RequirementStatus");
        }

        private async Task LoadIndexAsync()
        {
            IEnumerable<Requirement> data = null;
            if (User.Current.IsLoggedIn)
            {
                try
                {
                    data = await _requirementDataService.GetModelDataAsync(
                        User.Current.AuthToken, FeedbackActions.ReactToException);
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                    throw;
                }
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

        public ICommand NewCmd { get; }

        public ICommand EditCmd { get; }

        public ICommand DeleteCmd { get; }

        public ICommand SaveCmd { get; }

        public ICommand CancelCmd { get; }

        // TODO: Add permissions check on current user
        public bool CanExecute => !ServiceLocator.Current.GetInstance<MainViewModel>().IsBusy;

        // TODO: Add permissions check on current user
        public bool CanExecuteNew => CanExecute && CanSelectItem;

        // TODO: Add permissions check on current user
        public bool CanExecuteSelectedItemChange => CanExecute && !IsSelectedItemEditable;

        public bool CanExecuteCancel => IsSelectedItemEditable;

        public bool CanExecuteSave => CanExecute && IsSelectedItemEditable && SelectedItem.IsDirty;

        public ObservableCollection<Requirement> Index { get; }

        public bool IsSelectedItemEditable => SelectedItem != null && SelectedItem.IsLockedForEditing;

        public bool CanSelectItem
        {
            get => _canSelectItem;
            set => Set(ref _canSelectItem, value);
        }

        public Requirement SelectedItem
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

        public Lookup EstimationLookup
        {
            get => _estimationLookup;
            private set => Set(ref _estimationLookup, value);
        }

        public Lookup PriorityLookup
        {
            get => _priorityLookup;
            private set => Set(ref _priorityLookup, value);
        }

        public Lookup StatusLookup
        {
            get => _statusLookup;
            private set => Set(ref _statusLookup, value);
        }

        public string StoryFilter
        {
            get => _storyFilter;
            set => Set(ref _storyFilter, value);
        }

        private void OnFindStory(StoryFilterMessage m)
        {
            StoryFilter = m.Filter;
        }

        private async void OnNew()
        {
            MessengerInstance.Send(new BusyMessage(true));
            try
            {
                SelectedItem = await _requirementDataService.CreateModelAsync(
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
                MessengerInstance.Send(new BusyMessage(false));
            }            
        }

        private async void OnEdit()
        {
            MessengerInstance.Send(new BusyMessage(true));
            try
            {
                SelectedItem = await _requirementDataService.EditModelAsync(
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
                MessengerInstance.Send(new BusyMessage(false));
            }
        }

        private async void OnDelete()
        {
            var confirmed = ActionConfirmation.Confirm(ActionConfirmations.Delete);
            if (confirmed)
            {
                MessengerInstance.Send(new BusyMessage(true));
                bool result;
                try
                {
                    result = await _requirementDataService.DeleteModelAsync(
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
                    MessengerInstance.Send(new BusyMessage(false));
                }
            }
        }

        private async void OnSave()
        {
            MessengerInstance.Send(new BusyMessage(true));
            try
            {
                Requirement updated;
                if (SelectedItem.IsNew)
                {
                    updated = await _requirementDataService.StoreModelAsync(
                        User.Current.AuthToken, FeedbackActions.ReactToException, SelectedItem);
                }
                else
                {
                    updated = await _requirementDataService.UpdateModelAsync(
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
                MessengerInstance.Send(new BusyMessage(false));
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
                MessengerInstance.Send(new BusyMessage(true));
                await LoadIndexAsync();
                CanSelectItem = true;
                SelectedItem = Index.FirstOrDefault();
                MessengerInstance.Send(new BusyMessage(false));
            }
        }
    }
}
