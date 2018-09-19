// Pre Standard .Net (see http://www.mvvmlight.net/std10) using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using SixNations.Desktop.Interfaces;
using System.Threading.Tasks;
using SixNations.Desktop.Models;
using SixNations.Data.Models;

namespace SixNations.Desktop.Services
{
    public class LookupDataService : IHttpDataService<Lookup>
    {
        private IEnumerable<Lookup> _lookup;

        private async Task<Lookup> GetLookup(string lookupName)
        {
            var response = await SimpleIoc.Default.GetInstance<IHttpDataServiceFacade>()
                .HttpRequestAsync(lookupName.ToLower(), User.Current.AuthToken);
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

        public Task<Lookup> CreateModelAsync(string authToken, Action<Exception> exceptionHandler)
        {
            throw new NotSupportedException();
        }

        public Task<Lookup> StoreModelAsync(
            string authToken, Action<Exception> exceptionHandler, Lookup model)
        {
            throw new NotSupportedException();
        }

        public Task<Lookup> UpdateModelAsync(
            string authToken, Action<Exception> exceptionHandler, Lookup model)
        {
            throw new NotSupportedException();
        }

        public Task<Lookup> EditModelAsync(
            string authToken, Action<Exception> exceptionHandler, int modelId)
        {
            throw new NotSupportedException();
        }

        public Task<Lookup> EditModelAsync(
            string authToken, Action<Exception> exceptionHandler, Lookup model)
        {
            throw new NotSupportedException();
        }

        public Task<bool> DeleteModelAsync(
            string authToken, Action<Exception> exceptionHandler, Lookup model)
        {
            throw new NotSupportedException();
        }
    }
}
