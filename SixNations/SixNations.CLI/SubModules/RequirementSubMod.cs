using System;
using System.Linq;
using System.Threading.Tasks;
using SixNations.API.Interfaces;
using SixNations.CLI.Interfaces;
using SixNations.CLI.IO;
using SixNations.CLI.Modules;
using SixNations.Data.Models;

namespace SixNations.CLI.SubModules
{
    public class RequirementSubMod : BaseModule, ISubModule
    {
        private readonly IDataService<Requirement> _requirementsDataService;
        private readonly IDataService<Lookup> _lookupsDataService;
        private Lookup _estimationLookup;
        private Lookup _priorityLookup;
        private bool _isEscaped;

        public RequirementSubMod(
            IInputEntryService entryService,
            IDataService<Requirement> requirementsDataService, 
            IDataService<Lookup> lookupsDataService,
            Requirement selected = null)
            : base(entryService)
        {
            _requirementsDataService = requirementsDataService;
            _lookupsDataService = lookupsDataService;
            Selected = selected;

            Entry.Escaped += OnEscaped;
        }

        private void OnEscaped(object sender, EventArgs e)
        {
            _isEscaped = true;
        }

        public Requirement Selected { get; private set; }

        public async Task RunAsync()
        {
            if (!User.Current.IsLoggedIn)
            {
                return;
            }
            await LoadLookupsAsync();
            if (Selected is null)
            {
                Selected = await _requirementsDataService.CreateModelAsync(
                    User.Current.AuthToken, 
                    (ex) => Feedback.Show(ex, Formats.Danger));
            }
            else
            {
                Selected = await _requirementsDataService.EditModelAsync(
                    User.Current.AuthToken, 
                    (ex) => Feedback.Show(ex, Formats.Danger),
                    Selected);
            }
            RunForm();
            if (_isEscaped)
            {
                return;
            }
            if (Selected.IsNew)
            {
                Selected = await _requirementsDataService.StoreModelAsync(
                    User.Current.AuthToken,
                    (ex) => Feedback.Show(ex, Formats.Danger),
                    Selected);
            }
            else
            {
                Selected = await _requirementsDataService.UpdateModelAsync(
                    User.Current.AuthToken,
                    (ex) => Feedback.Show(ex, Formats.Danger),
                    Selected);
            }
            Feedback.Show(true);
        }

        private async Task LoadLookupsAsync()
        {
            var lookups = await _lookupsDataService.GetModelDataAsync(
                        User.Current.AuthToken, (ex) => Feedback.Show(ex, Formats.Danger));

            _estimationLookup = lookups.First(l => l.Name == "RequirementEstimation");
            _priorityLookup = lookups.First(l => l.Name == "RequirementPriority");
        }

        private void RunForm()
        {
            Selected.Story = Entry.Read("Story");
            if (_isEscaped)
            {
                return;
            }          
            var lookup = $"Estimation (ID) {_estimationLookup.ToJson()}";
            var entry = Entry.Read(lookup);
            if (_isEscaped)
            {
                return;
            }
            int.TryParse(entry, out int estimation);
            int[] estimations = { 1, 2, 3, 5, 8, 13 };            
            Selected.Estimation = estimations.Where(e => e == estimation).FirstOrDefault();

            lookup = $"Priority (ID) {_priorityLookup.ToJson()}";
            entry = Entry.Read(lookup);
            if (_isEscaped)
            {
                return;
            }
            int.TryParse(entry, out int priority);
            if (priority > 0 && priority < 5)
            {
                Selected.Priority = priority;
            }

            lookup = "Status (ID) " +
                "{\"1\": \"Prioritised\", \"2\": \"WIP\", \"3\": \"Test\", \"4\": \"Done\"}";
            entry = Entry.Read(lookup);
            if (_isEscaped)
            {
                return;
            }
            int.TryParse(entry, out int status);
            if (status > 0 && status < 5)
            {
                Selected.Status = status;
            }
            Selected.Release = Entry.Read("Release");
        }
    }
}
