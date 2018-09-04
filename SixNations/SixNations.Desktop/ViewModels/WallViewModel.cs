using System.Linq;
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
            Prioritised = new RequirementStatusSwimlane(RequirementStatus.Prioritised);
            WIP = new RequirementStatusSwimlane(RequirementStatus.WIP);
            Test = new RequirementStatusSwimlane(RequirementStatus.Test);
            Done = new RequirementStatusSwimlane(RequirementStatus.Done);
        }

        public async override Task LoadAsync()
        {
            await base.LoadAsync();

            Prioritised.Index.Clear();
            Index.Where(r => r.Status == (int)RequirementStatus.Prioritised)
                .ToList().ForEach(r => Prioritised.Index.Add(r));

            WIP.Index.Clear();
            Index.Where(r => r.Status == (int)RequirementStatus.WIP)
                .ToList().ForEach(r => WIP.Index.Add(r));

            Test.Index.Clear();
            Index.Where(r => r.Status == (int)RequirementStatus.Test)
                .ToList().ForEach(r => Test.Index.Add(r));

            Done.Index.Clear();
            Index.Where(r => r.Status == (int)RequirementStatus.Done)
                .ToList().ForEach(r => Done.Index.Add(r));
        }

        public RequirementStatusSwimlane Prioritised { get; }

        public RequirementStatusSwimlane WIP { get; }

        public RequirementStatusSwimlane Test { get; }

        public RequirementStatusSwimlane Done { get; }
    }
}
