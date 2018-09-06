using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SixNations.Desktop.Helpers;
using SixNations.Desktop.Interfaces;
using SixNations.Desktop.Messages;
using SixNations.Desktop.Models;
using SixNations.Desktop.Constants;

namespace SixNations.Desktop.ViewModels
{
    public class RequirementViewModel : DataBoundViewModel<Requirement>
    {
        private static readonly ILog Log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IDataService<Lookup> _lookupDataService;
        private Lookup _estimationLookup;
        private Lookup _priorityLookup;
        private Lookup _statusLookup;
        private string _storyFilter;

        public RequirementViewModel(
            IDataService<Requirement> requirementDataService,
            IDataService<Lookup> lookupService)
            : base(requirementDataService)
        {
            _storyFilter = string.Empty;

            _lookupDataService = lookupService;

            MessengerInstance.Register<StoryFilterMessage>(this, OnFindStory);
        }

        public override async Task LoadAsync()
        {
            MessengerInstance.Send(new BusyMessage(true, this));
            await base.LoadAsync();
            try
            {
                await LoadLookupAsync();
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Unexpexted exception loading! {0}", ex);
                FeedbackActions.ReactToException(ex);
            }
            finally
            {
                MessengerInstance.Send(new BusyMessage(false, this));
            }
        }

        private async Task LoadLookupAsync()
        {
            var lookups = await _lookupDataService.GetModelDataAsync(
                        User.Current.AuthToken, FeedbackActions.ReactToException);

            EstimationLookup = lookups.First(l => l.Name == "RequirementEstimation");
            PriorityLookup = lookups.First(l => l.Name == "RequirementPriority");
            StatusLookup = new Lookup(RequirementStatus._);
        }

        public Lookup EstimationLookup
        {
            get => _estimationLookup;
            private set => Set(ref _estimationLookup, value);
        }

        public Lookup PriorityLookup
        {
            get => _priorityLookup;
            private set => Set(ref _priorityLookup, value);
        }

        public Lookup StatusLookup
        {
            get => _statusLookup;
            private set => Set(ref _statusLookup, value);
        }

        public string StoryFilter
        {
            get => _storyFilter;
            set => Set(ref _storyFilter, value);
        }

        private void OnFindStory(StoryFilterMessage m)
        {
            StoryFilter = m.Filter;
        }
    }
}
