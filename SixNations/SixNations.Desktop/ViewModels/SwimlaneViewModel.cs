using System.Linq;
using GalaSoft.MvvmLight;
using SixNations.Desktop.Constants;
using System.Collections.ObjectModel;
using SixNations.Desktop.Interfaces;
using SixNations.Desktop.Messages;
using SixNations.Desktop.Models;
using SixNations.Desktop.Helpers;

namespace SixNations.Desktop.ViewModels
{
    public class SwimlaneViewModel : ViewModelBase, ISwimlaneDropTarget
    {
        private readonly IDataService<Requirement> _requirementDataService;

        public SwimlaneViewModel(
            IDataService<Requirement> requirementDataService,
            RequirementStatus name)
        {
            Name = name;
            Index = new ObservableCollection<IRequirement>();
            _requirementDataService = requirementDataService;
        }

        public RequirementStatus Name { get; }

        public ObservableCollection<IRequirement> Index { get; }

        public async void OnDrop(int droppedRequirementId, RequirementStatus target)
        {
            var requirement = Index.Where(r => r.Id == droppedRequirementId).FirstOrDefault();
            var isDroppedOnSameSwimlane = requirement != null;
            if (!isDroppedOnSameSwimlane)
            {
                MessengerInstance.Send(new BusyMessage(true, this));

                var authToken = User.Current.AuthToken;

                var dropped = await _requirementDataService.EditModelAsync(
                    authToken, FeedbackActions.ReactToException, droppedRequirementId);

                dropped.Status = (int)target;

                var updated = await _requirementDataService.UpdateModelAsync(
                    authToken, FeedbackActions.ReactToException, dropped as Requirement);
                MessengerInstance.Send(new BusyMessage(false, this));

                MessengerInstance.Send(new ReloadRequestMessage(this));
            }
        }
    }
}
