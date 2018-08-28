using System.Collections.Generic;
using System.Threading.Tasks;
using SixNations.Desktop.Models;

namespace SixNations.Desktop.Interfaces
{
    public interface IHttpDataServiceFacade
    {
        Task<ResponseRootObject> HttpRequestAsync(string uri, string token);
        Task<ResponseRootObject> HttpRequestAsync(string uri, string token, HttpMethods httpMethod, IDictionary<string, object> data);
    }
}