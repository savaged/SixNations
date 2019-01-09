using System.Collections.Generic;
using System.Threading.Tasks;
using SixNations.API.Constants;

namespace SixNations.API.Interfaces
{
    public interface IHttpDataServiceFacade
    {
        Task<IResponseRootObject> HttpRequestAsync(string uri, string token);
        Task<IResponseRootObject> HttpRequestAsync(string uri, string token, HttpMethods httpMethod, IDictionary<string, object> data);
    }
}