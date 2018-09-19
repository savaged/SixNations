using System.Linq;
using GalaSoft.MvvmLight;
using SixNations.Desktop.Constants;
using System.Collections.ObjectModel;
using SixNations.Desktop.Interfaces;
using SixNations.Desktop.Messages;
using SixNations.Data.Models;
using SixNations.Desktop.Helpers;
using SixNations.API.Interfaces;

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
            Index = new ObservableCollection<PostItViewModel>();
            _requirementDataService = requirementDataService;
        }

        public RequirementStatus Name { get; }

        public ObservableCollection<PostItViewModel> Index { get; }        

        public async void OnDrop(int droppedRequirementId, RequirementStatus target)
        {
            var postIt = Index.Where(r => r.Requirement.Id == droppedRequirementId).FirstOrDefault();
            var isDroppedOnSameSwimlane = postIt != null;
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
