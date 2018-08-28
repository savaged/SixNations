using System;
using System.Collections.Generic;
using SixNations.Desktop.Interfaces;
using System.Threading.Tasks;
using SixNations.Desktop.Models;
using CommonServiceLocator;

namespace SixNations.Desktop.Services
{
    public class LookupDataService : IHttpDataService<Lookup>
    {
        private IEnumerable<Lookup> _lookup;

        private async Task<Lookup> GetLookup(string lookupName)
        {
            var response = await ServiceLocator.Current.GetInstance<IHttpDataServiceFacade>()
                .HttpRequestAsync(
                lookupName.ToLower(), User.Current.AuthToken);
            return new Lookup(lookupName, response);
        }

        public async Task<IEnumerable<Lookup>> GetModelDataAsync(
            string authToken, Action<Exception> exceptionHandler)
        {
            if (_lookup == null)
            {
                _lookup = new List<Lookup>
                {
                    await GetLookup("RequirementEstimation"),
                    await GetLookup("RequirementPriority"),
                    await GetLookup("RequirementStatus")
                };
            }
            return _lookup;
        }
    }
}
