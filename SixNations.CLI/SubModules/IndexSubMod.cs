using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using SixNations.API.Interfaces;
using SixNations.CLI.Interfaces;
using SixNations.CLI.IO;
using SixNations.Data.Models;

namespace SixNations.CLI.Modules
{
    public class IndexSubMod : BaseModule, ISubModule
    {
        private readonly IDataService<Requirement> _dataService;
        private readonly IList<Requirement> _index;

        public IndexSubMod(IDataService<Requirement> dataService)
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
            var first = _index.FirstOrDefault();
            Feedback.Show(first);
            // TODO feedback page fwd/back, search and CRUD
        }

        private async Task LoadIndexAsync()
        {
            var data = await _dataService.GetModelDataAsync(
                    User.Current.AuthToken, (ex) => Feedback.Show(ex, Formats.Danger));
            foreach (var item in data)
            {
                _index.Add(item);
            }
        }
    }
}
