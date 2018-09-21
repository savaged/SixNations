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
        private readonly IDataService<Requirement> _dataService;
        // TODO add lookups

        public RequirementSubMod(
            IDataService<Requirement> dataService, Requirement selected = null)
        {
            _dataService = dataService;
            Selected = selected;
        }

        public Requirement Selected { get; private set; }

        public async Task RunAsync()
        {
            if (!User.Current.IsLoggedIn)
            {
                return;
            }
            if (Selected == null)
            {
                Selected = await _dataService.CreateModelAsync(
                    User.Current.AuthToken, 
                    (ex) => Feedback.Show(ex, Formats.Danger));
            }
            else
            {
                Selected = await _dataService.EditModelAsync(
                    User.Current.AuthToken, 
                    (ex) => Feedback.Show(ex, Formats.Danger),
                    Selected);
            }
            RunForm();
            if (Selected.IsNew)
            {
                Selected = await _dataService.StoreModelAsync(
                    User.Current.AuthToken,
                    (ex) => Feedback.Show(ex, Formats.Danger),
                    Selected);
            }
            else
            {
                Selected = await _dataService.UpdateModelAsync(
                    User.Current.AuthToken,
                    (ex) => Feedback.Show(ex, Formats.Danger),
                    Selected);
            }
            Feedback.Show(true);
        }

        private void RunForm()
        {
            Selected.Story = Entry.Read("Story");

            var lookup = "Estimation ('1' XS; '2' Small; '3' Medium; '5' Large; '8' XL; '13' XXL)";
            int.TryParse(Entry.Read(lookup), out int estimation);
            int[] estimations = { 1, 2, 3, 5, 8, 13 };            
            Selected.Estimation = estimations.Where(e => e == estimation).FirstOrDefault();

            lookup = "Priority ('1' Must; '2' Should; '3' Could; '4' Won't)";
            int.TryParse(Entry.Read(lookup), out int priority);
            if (priority > 0 && priority < 5)
            {
                Selected.Priority = priority;
            }

            lookup = "Status ('1' Prioritised; '2' WIP; '3' Test; '4' Done)";
            int.TryParse(Entry.Read(lookup), out int status);
            if (status > 0 && status < 5)
            {
                Selected.Status = status;
            }

            Selected.Release = Entry.Read("Release");
        }
    }
}
