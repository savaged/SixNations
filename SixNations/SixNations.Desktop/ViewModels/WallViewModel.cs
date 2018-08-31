using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SixNations.Desktop.Interfaces;
using SixNations.Desktop.Models;
using SixNations.Desktop.Constants;

namespace SixNations.Desktop.ViewModels
{
    public class WallViewModel : DataBoundViewModel<Requirement>
    {
        public WallViewModel(IDataService<Requirement> dataService) 
            : base(dataService)
        {
            Prioritised = new ObservableCollection<Requirement>();
            WIP = new ObservableCollection<Requirement>();
            Test = new ObservableCollection<Requirement>();
            Done = new ObservableCollection<Requirement>();
        }

        public async override Task LoadAsync()
        {
            await base.LoadAsync();

            Index.Where(r => r.Status == (int)RequirementStatus.Prioritised)
                .ToList().ForEach(r => Prioritised.Add(r));

            Index.Where(r => r.Status == (int)RequirementStatus.WIP)
                .ToList().ForEach(r => WIP.Add(r));

            Index.Where(r => r.Status == (int)RequirementStatus.Test)
                .ToList().ForEach(r => Test.Add(r));

            Index.Where(r => r.Status == (int)RequirementStatus.Done)
                .ToList().ForEach(r => Done.Add(r));
        }

        public ObservableCollection<Requirement> Prioritised { get; }

        public ObservableCollection<Requirement> WIP { get; }

        public ObservableCollection<Requirement> Test { get; }

        public ObservableCollection<Requirement> Done { get; }
    }
}
