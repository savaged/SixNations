using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using log4net;
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

        public RequirementViewModel(IRequirementDataService requirementDataService)
        {
            _requirementDataService = requirementDataService;
            if (IsInDesignMode)
            {
                _ = LoadAsync();
            }
        }

        public async Task LoadAsync()
        {
            IEnumerable<Requirement> data = null;
            try
            {
                data = await _requirementDataService.GetModelDataAsync(null, null);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
            if (data != null)
            {
                Index = new ObservableCollection<Requirement>(data);
            }
            else
            {
                Index = new ObservableCollection<Requirement>();
            }
        }

        public ObservableCollection<Requirement> Index { get; private set; }
    }
}
