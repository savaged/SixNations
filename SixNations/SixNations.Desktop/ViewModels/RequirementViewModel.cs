using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
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
        private Lookup _estimationLookup;
        private Lookup _priorityLookup;
        private Lookup _statusLookup;

        public RequirementViewModel(
            IDataService<Requirement> requirementDataService,
            IDataService<Lookup> lookupService)
        {
            _requirementDataService = requirementDataService;
            _lookupDataService = lookupService;

            Index = new ObservableCollection<Requirement>();
            if (IsInDesignMode)
            {
                _ = LoadAsync();
            }
        }

        public async Task LoadAsync()
        {
            MessengerInstance.Send(new BusyMessage(true));

            await LoadLookupAsync();

            IEnumerable<Requirement> data = null;
            if (IsInDesignMode)
            {
                data = await _requirementDataService.GetModelDataAsync(null, null);
            }
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
                SelectedItem = Index.First();
            }
            else
            {
                Index.Clear();
            }
            MessengerInstance.Send(new BusyMessage(false));
        }

        private async Task LoadLookupAsync()
        {
            IEnumerable<Lookup> lookups = null;
            if (IsInDesignMode)
            {
                lookups = await _lookupDataService.GetModelDataAsync(null, null);
            }
            else
            {
                lookups = await _lookupDataService.GetModelDataAsync(
                        User.Current.AuthToken, FeedbackActions.ReactToException);
            }
            EstimationLookup = lookups.First(l => l.Name == "RequirementEstimation");
            PriorityLookup = lookups.First(l => l.Name == "RequirementPriority");
            StatusLookup = lookups.First(l => l.Name == "RequirementStatus");
        }

        public ObservableCollection<Requirement> Index { get; }

        public Requirement SelectedItem
        {
            get => _selectedItem;
            set => Set(ref _selectedItem, value);
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
    }
}
