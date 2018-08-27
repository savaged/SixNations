using System;
using System.Collections.Generic;
using SixNations.Desktop.Interfaces;
using System.Threading.Tasks;
using SixNations.Desktop.Models;

namespace SixNations.Desktop.Services
{
    public class DesignLookupDataService : IHttpDataService<Lookup>
    {
        private Lookup Show(string lookupName)
        {
            var dict = new Dictionary<int, string>();
            switch (lookupName)
            {
                case "Estimation":
                    GetEstimationLookup();
                    break;
                case "Priority":
                    GetPriorityLookup();
                    break;
                case "Status":
                    GetStatusLookup();
                    break;
            }
            var data = new Lookup(dict);
            return data;
        }

        private IDictionary<int, string> GetEstimationLookup()
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
            return dict;
        }

        private IDictionary<int, string> GetPriorityLookup()
        {
            var dict = new Dictionary<int, string>
            {
                { 1, "Must" },
                { 2, "Should" },
                { 3, "Could" },
                { 4, "Wont" },
            };
            return dict;
        }

        private IDictionary<int, string> GetStatusLookup()
        {
            var dict = new Dictionary<int, string>
            {
                { 1, "Prioritised" },
                { 2, "WIP" },
                { 3, "Test" },
                { 4, "Done" },
            };
            return dict;
        }

        public async Task<IEnumerable<Lookup>> GetModelDataAsync(
            string authToken, Action<Exception> exceptionHandler)
        {
            await Task.CompletedTask;

            var col = new List<Lookup>
            {
                Show("Estimation"),
                Show("Priority"),
                Show("Status")
            };
            return col;
        }
    }
}
