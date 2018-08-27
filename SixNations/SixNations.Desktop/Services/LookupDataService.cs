using System;
using System.Collections.Generic;
using SixNations.Desktop.Interfaces;
using System.Threading.Tasks;
using SixNations.Desktop.Models;

namespace SixNations.Desktop.Services
{
    public class LookupDataService : IHttpDataService<Lookup>
    {
        private readonly string _lookupName;

        public LookupDataService(string lookupName)
        {
            _lookupName = lookupName;
        }

        public Task<IEnumerable<Lookup>> GetModelDataAsync(
            string authToken, Action<Exception> exceptionHandler)
        {
            throw new NotImplementedException();
        }
    }
}
