using System.Collections.Generic;
using System.Threading.Tasks;
using SixNations.Desktop.Constants;
using SixNations.Desktop.Models;

namespace SixNations.Desktop.Interfaces
{
    public interface IHttpDataServiceFacade
    {
        Task<IResponseRootObject> HttpRequestAsync(string uri, string token);
        Task<IResponseRootObject> HttpRequestAsync(string uri, string token, API.Constants.HttpMethods httpMethod, IDictionary<string, object> data);
    }
}