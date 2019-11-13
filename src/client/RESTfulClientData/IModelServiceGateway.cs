using System.Collections.Generic;
using System.Threading.Tasks;

namespace savaged.mvvm.Data
{
    public interface IModelServiceGateway
    {
        string BaseUrl { get; }

        ApiSettings ApiSettings { get; }

        Task<bool> ValueAsync(
            IAuthUser user,
            string uri,
            HttpMethods httpMethod);

        Task<object> ValueAsync(IAuthUser user, string uri);

        Task<IResponseFileStream> GetFileAsync(
            IAuthUser user, string uri);

        Task<IEnumerable<T>> GetIndexAsync<T>(
            IAuthUser user,
            string uri,
            IDictionary<string, object> data = null)
            where T : IDataModel;

        Task<T> GetObjectAsync<T>(IAuthUser user, string uri)
            where T : IDataModel;


        Task<T> ScalarDataAsync<T>(
            IAuthUser user,
            string uri,
            HttpMethods httpMethod,
            IDictionary<string, object> data = null)
            where T : IDataModel;

        Task<IEnumerable<T>> IndexDataAsync<T>(
            IAuthUser user,
            string uri,
            HttpMethods httpMethod,
            IDictionary<string, object> data = null)
            where T : IDataModel;
    }
}