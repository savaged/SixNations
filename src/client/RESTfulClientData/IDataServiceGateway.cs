using System.Collections.Generic;
using System.Threading.Tasks;

namespace savaged.mvvm.Data
{
    public interface IDataServiceGateway
    {
        string BaseUrl { get; }

        ApiSettings ApiSettings { get; }

        Task<string> DataAsync(
            IAuthUser user, 
            string uri, 
            HttpMethods httpMethod, 
            IDictionary<string, object> data = null);

        Task<IResponseFileStream> GetFileAsync(
            IAuthUser user, string uri);

        Task<string> GetIndexAsync(
            IAuthUser user, 
            string uri, 
            IDictionary<string, object> data = null);

        Task<string> GetObjectAsync(IAuthUser user, string uri);

    }
}