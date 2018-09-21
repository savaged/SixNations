using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using SixNations.API.Interfaces;
using SixNations.CLI.Interfaces;
using SixNations.CLI.IO;
using SixNations.Data.Models;
using SixNations.CLI.Modules;

namespace SixNations.CLI.SubModules
{
    public class RequirementsSubMod : BaseModule, ISubModule
    {
        private readonly IDataService<Requirement> _dataService;
        private readonly List<Requirement> _index;
        private Requirement _selected;
        private bool _isQuitRequested;

        public RequirementsSubMod(IDataService<Requirement> dataService)
        {
            _dataService = dataService;
            _index = new List<Requirement>();
        }

        public async Task RunAsync()
        {
            if (!User.Current.IsLoggedIn)
            {
                return;
            }
            if (_index.Count == 0)
            {
                await LoadIndexAsync();
            }
            _selected = _index.FirstOrDefault();
            ShowSelected();

            const string actions = 
                "Action ('<' previous; '>' next; 's' search;" +
                " 'n' new; 'e' edit; 'd' delete; 'q' quit)";

            while (!_isQuitRequested)
            {
                var action = Entry.Read(actions);
                await RunAsync(action);
            }
        }

        private async Task RunAsync(string action)
        {
            var token = User.Current.AuthToken;
            switch (action)
            {
                case ">":
                    SetSelectedToNext();
                    ShowSelected();
                    break;
                case "<":
                    SetSelectedToPrevious();
                    ShowSelected();
                    break;
                case "s":
                    var searchArg = Entry.Read("Search (ID or Word[s] in Story)");
                    SetSelectedToSearchArg(searchArg);
                    break;
                case "n":
                    var creator = new RequirementSubMod(_dataService);
                    await creator.RunAsync();
                    await LoadIndexAsync();
                    _selected = creator.Selected;
                    ShowSelected();
                    break;
                case "e":
                    var editor = new RequirementSubMod(_dataService, _selected);
                    await editor.RunAsync();
                    await LoadIndexAsync();
                    _selected = editor.Selected;
                    ShowSelected();
                    break;
                case "d":
                    var result = await _dataService.DeleteModelAsync(
                        token, (ex) => Feedback.Show(ex), _selected);
                    Feedback.Show(result);
                    if (result)
                    {
                        await LoadIndexAsync();
                    }
                    break;
                case "q":
                    _isQuitRequested = true;
                    break;
            }
        }

        private async Task LoadIndexAsync()
        {
            var data = await _dataService.GetModelDataAsync(
                    User.Current.AuthToken, (ex) => Feedback.Show(ex, Formats.Danger));
            _index.Clear();
            foreach (var item in data)
            {
                _index.Add(item);
            }
        }

        private void SetSelectedToSearchArg(string searchArg)
        {
            var isId = int.TryParse(searchArg, out int id);
            Requirement match = null;
            if (isId)
            {
                match = _index.Where(r => r.Id == id).FirstOrDefault();
            }
            else
            {
                match = _index.Where(r => r.Story.Contains(searchArg)).FirstOrDefault();
            }
            if (match != null)
            {
                _selected = match;
                Feedback.Show(_selected);
            }
            else
            {
                Feedback.Show("No match!", Formats.Warning);
            }
        }

        private int SelectedPosition => _index.FindIndex(r => r == _selected);

        private void SetSelectedToNext()
        {
            var pos = SelectedPosition + 1;
            if (pos == _index.Count)
            {
                pos = 0;
            }
            _selected = _index[pos];
        }

        private void SetSelectedToPrevious()
        {
            var pos = SelectedPosition - 1;
            if (pos < 0)
            {
                pos = _index.Count - 1;
            }
            _selected = _index[pos];
        }

        private void ShowSelected()
        {
            Feedback.Show(_selected);
        }
    }
}
