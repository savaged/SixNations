using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using log4net;
using SixNations.Desktop.Helpers;
using SixNations.Desktop.Interfaces;
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
        }

        private async Task LoadLookupAsync()
        {
            if (IsInDesignMode)
            {
                var lookups = await _lookupDataService.GetModelDataAsync(
                    User.Current.AuthToken, FeedbackActions.ReactToException);

                EstimationLookup = lookups.First(l => l.Name == "Estimation");
                PriorityLookup = lookups.First(l => l.Name == "Priority");
                StatusLookup = lookups.First(l => l.Name == "Status");
            }
        }

        public ObservableCollection<Requirement> Index { get; }

        public Requirement SelectedItem
        {
            get => _selectedItem;
            set => Set(ref _selectedItem, value);
        }

        public Lookup EstimationLookup { get; private set; }

        public Lookup PriorityLookup { get; private set; }

        public Lookup StatusLookup { get; private set; }
    }
}
