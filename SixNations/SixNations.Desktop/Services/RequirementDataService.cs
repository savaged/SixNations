using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SixNations.Desktop.Facade;
using SixNations.Desktop.Helpers;
using SixNations.Desktop.Interfaces;
using SixNations.Desktop.Models;

namespace SixNations.Desktop.Services
{
    public class RequirementDataService : IHttpDataService<Requirement>
    {
        public async Task<IEnumerable<Requirement>> GetModelDataAsync(
            string authToken, Action<Exception> exceptionHandler)
        {
            var uri = typeof(Requirement).NameToUriFormat();

            ResponseRootObject response = null;
            try
            {
                response = await HttpDataServiceFacade.HttpRequestAsync(uri, authToken);
            }
            catch (Exception ex)
            {
                exceptionHandler(ex);
                response = new ResponseRootObject(ex.Message);
            }
            var index = new ResponseRootObjectToModelMapper<Requirement>(response).AllMapped();
            return index;
        }
    }
}
