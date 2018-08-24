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
        private readonly IRequirementDataService _requirementDataService;
        private Requirement _selectedItem;

        public RequirementViewModel(IRequirementDataService requirementDataService)
        {
            _requirementDataService = requirementDataService;
            Index = new ObservableCollection<Requirement>();
            if (IsInDesignMode)
            {
                _ = LoadAsync();
            }
        }

        public async Task LoadAsync()
        {
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

        public ObservableCollection<Requirement> Index { get; }

        public Requirement SelectedItem
        {
            get => _selectedItem;
            set => Set(ref _selectedItem, value);
        }
    }
}
