using System.Linq;
using System.Threading.Tasks;
using Savaged.BusyStateManager;
using SixNations.API.Interfaces;
using SixNations.Data.Models;
using SixNations.API.Constants;
using SixNations.Desktop.Messages;
using SixNations.Desktop.Interfaces;
using SixNations.Desktop.Services;
using GalaSoft.MvvmLight.Threading;

namespace SixNations.Desktop.ViewModels
{
    public class WallViewModel : DataBoundViewModel<Requirement>
    {
        public WallViewModel(
            IDataService<Requirement> requirementDataService,
            IActionConfirmationService actionConfirmation) 
            : base(requirementDataService, actionConfirmation)
        {
            Prioritised = new SwimlaneViewModel(requirementDataService, RequirementStatus.Prioritised);
            WIP = new SwimlaneViewModel(requirementDataService, RequirementStatus.WIP);
            Test = new SwimlaneViewModel(requirementDataService, RequirementStatus.Test);
            Done = new SwimlaneViewModel(requirementDataService, RequirementStatus.Done);

            PollingService.Instance.IntervalElapsed += OnPollingIntervalElapsed;
            MessengerInstance.Register<ReloadRequestMessage>(this, OnReloadRequest);
        }

        public override void Cleanup()
        {
            PollingService.Instance.Stop();
            PollingService.Instance.IntervalElapsed -= OnPollingIntervalElapsed;
            base.Cleanup();
        }

        public async override Task LoadAsync()
        {
            MessengerInstance.Send(new BusyMessage(true, this));

            await base.LoadAsync();

            const int wont = 4;
            var filtered = Index.Where(r => r.Priority != wont);

            Prioritised.Index.Clear();
            filtered.Where(r => r.Status == (int)RequirementStatus.Prioritised)
                .ToList().ForEach(r => Prioritised.Index.Add(new PostItViewModel(r)));

            WIP.Index.Clear();
            filtered.Where(r => r.Status == (int)RequirementStatus.WIP)
                .ToList().ForEach(r => WIP.Index.Add(new PostItViewModel(r)));

            Test.Index.Clear();
            filtered.Where(r => r.Status == (int)RequirementStatus.Test)
                .ToList().ForEach(r => Test.Index.Add(new PostItViewModel(r)));

            Done.Index.Clear();
            filtered.Where(r => r.Status == (int)RequirementStatus.Done)
                .ToList().ForEach(r => Done.Index.Add(new PostItViewModel(r)));

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

        private void OnPollingIntervalElapsed()
        {
            DispatcherHelper.CheckBeginInvokeOnUI(async () =>
            {
                await LoadAsync();
            });
        }
    }
}
