using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using log4net;
using SixNations.Desktop.Interfaces;
using SixNations.Desktop.Models;

namespace SixNations.Desktop.ViewModels
{
    public class RequirementViewModel : ViewModelBase
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IRequirementDataService _requirementDataService;

        public RequirementViewModel(IRequirementDataService requirementDataService)
        {
            _requirementDataService = requirementDataService;
            // TODO Find a better way to do an async load or supress the warning
            Load();
        }

        private async Task Load()
        {
            IEnumerable<Requirement> data = null;
            try
            {
                data = await _requirementDataService.GetModelDataAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                // TODO Alert the UI of the error
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
