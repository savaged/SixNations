using System.Linq;
using System.Threading.Tasks;
using SixNations.Desktop.Interfaces;
using SixNations.Desktop.Models;
using SixNations.Desktop.Constants;
using GongSolutions.Wpf.DragDrop;
using SixNations.Desktop.Helpers;

namespace SixNations.Desktop.ViewModels
{
    public class WallViewModel : DataBoundViewModel<Requirement>
    {
        public WallViewModel(IDataService<Requirement> dataService) 
            : base(dataService)
        {
            Prioritised = new RequirementStatusSwimlane();
            WIP = new RequirementStatusSwimlane();
            Test = new RequirementStatusSwimlane();
            Done = new RequirementStatusSwimlane();

            DropHandler = new RequirementDropHandler();
        }

        public async override Task LoadAsync()
        {
            await base.LoadAsync();

            Index.Where(r => r.Status == (int)RequirementStatus.Prioritised)
                .ToList().ForEach(r => Prioritised.Index.Add(r));

            Index.Where(r => r.Status == (int)RequirementStatus.WIP)
                .ToList().ForEach(r => WIP.Index.Add(r));

            Index.Where(r => r.Status == (int)RequirementStatus.Test)
                .ToList().ForEach(r => Test.Index.Add(r));

            Index.Where(r => r.Status == (int)RequirementStatus.Done)
                .ToList().ForEach(r => Done.Index.Add(r));
        }

        public RequirementStatusSwimlane Prioritised { get; }

        public RequirementStatusSwimlane WIP { get; }

        public RequirementStatusSwimlane Test { get; }

        public RequirementStatusSwimlane Done { get; }

        public IDropTarget DropHandler { get; }
    }
}
