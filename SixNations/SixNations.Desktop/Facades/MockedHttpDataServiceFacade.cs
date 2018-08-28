using log4net;
using System;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using SixNations.Desktop.Interfaces;
using SixNations.Desktop.Models;
using SixNations.Desktop.Exceptions;
using Moq;

namespace SixNations.Desktop.Facade
{
    public class MockedHttpDataServiceFacade : IHttpDataServiceFacade
    {
        private static readonly ILog Log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Mock<IHttpDataServiceFacade> MockHttpDataServiceFacade { get; }

        public MockedHttpDataServiceFacade()
        {
            MockHttpDataServiceFacade = new Mock<IHttpDataServiceFacade>();
            // TODO MockHttpDataServiceFacade.Setup();
        }

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