using System.Linq;
using System.Threading.Tasks;
using SixNations.Desktop.Interfaces;
using SixNations.Desktop.Models;
using SixNations.Desktop.Constants;
using SixNations.Desktop.Messages;

namespace SixNations.Desktop.ViewModels
{
    public class WallViewModel : DataBoundViewModel<Requirement>
    {
        public WallViewModel(IDataService<Requirement> dataService) 
            : base(dataService)
        {
            Prioritised = new SwimlaneViewModel(dataService, RequirementStatus.Prioritised);
            WIP = new SwimlaneViewModel(dataService, RequirementStatus.WIP);
            Test = new SwimlaneViewModel(dataService, RequirementStatus.Test);
            Done = new SwimlaneViewModel(dataService, RequirementStatus.Done);
            MessengerInstance.Register<ReloadRequestMessage>(this, OnReloadRequest);
        }

        public async override Task LoadAsync()
        {
            MessengerInstance.Send(new BusyMessage(true, this));
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

            MessengerInstance.Send(new BusyMessage(false, this));
        }

        public SwimlaneViewModel Prioritised { get; }

        public SwimlaneViewModel WIP { get; }

        public SwimlaneViewModel Test { get; }

        public SwimlaneViewModel Done { get; }

        private async void OnReloadRequest(ReloadRequestMessage m)
        {
            await LoadAsync();
        }
    }
}
