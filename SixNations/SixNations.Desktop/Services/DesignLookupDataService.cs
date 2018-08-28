using System;
using System.Collections.Generic;
using SixNations.Desktop.Interfaces;
using System.Threading.Tasks;
using SixNations.Desktop.Models;

namespace SixNations.Desktop.Services
{
    public class DesignLookupDataService : IHttpDataService<Lookup>
    {
        private IEnumerable<Lookup> _lookup;

        private Lookup GetEstimationLookup()
        {
            var dict = new Dictionary<int, string>
            {
                { 1, "XS" },
                { 2, "Small" },
                { 3, "Medium" },
                { 5, "Large" },
                { 8, "XL" },
                { 13, "XXL" },
            };
            return new Lookup(dict);
        }

        private Lookup GetPriorityLookup()
        {
            var dict = new Dictionary<int, string>
            {
                { 1, "Must" },
                { 2, "Should" },
                { 3, "Could" },
                { 4, "Wont" },
            };
            return new Lookup(dict);
        }

        private Lookup GetStatusLookup()
        {
            var dict = new Dictionary<int, string>
            {
                { 1, "Prioritised" },
                { 2, "WIP" },
                { 3, "Test" },
                { 4, "Done" },
            };
            return new Lookup(dict);
        }

        private Lookup GetLookup(string lookupName)
        {
            Lookup lookup = null;
            switch (lookupName.ToLower())
            {
                case "requirementestimation":
                    lookup = GetEstimationLookup();
                    break;
                case "requirementpriority":
                    lookup = GetPriorityLookup();
                    break;
                case "requirementstatus":
                    lookup = GetStatusLookup();
                    break;
            }
            return lookup;
        }

        public async Task<IEnumerable<Lookup>> GetModelDataAsync(
            string authToken, Action<Exception> exceptionHandler)
        {
            await Task.CompletedTask;

            if (_lookup == null)
            {
                _lookup = new List<Lookup>
                {
                    GetLookup("RequirementEstimation"),
                    GetLookup("RequirementPriority"),
                    GetLookup("RequirementStatus")
                };
            }
            return _lookup;
        }
    }
}
