using log4net;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using SixNations.Desktop.Interfaces;
using SixNations.Desktop.Models;

namespace SixNations.Desktop.Facade
{
    public class MockedHttpDataServiceFacade : IHttpDataServiceFacade
    {
        private static readonly ILog Log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public async Task<ResponseRootObject> HttpRequestAsync(string uri, string token)
        {
            return await HttpRequestAsync(uri, token, HttpMethods.Get, null);
        }
        
        public async Task<ResponseRootObject> HttpRequestAsync(
            string uri, string token, HttpMethods httpMethod, IDictionary<string, object> data)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }
    }
}