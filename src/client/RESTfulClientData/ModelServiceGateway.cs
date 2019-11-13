using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace savaged.mvvm.Data
{
    public class ModelServiceGateway : IModelServiceGateway
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        private readonly IDataServiceGateway _dataServiceGateway;
        private readonly IList<string> _invalidResponseContent;

        public ModelServiceGateway(
            string baseUrl,
            ApiSettings apiSettings,
            JsonSerializerSettings jsonSerializerSettings = null,
            IApiFormatConverter apiFormatConverter = null,
            IList<string> invalidResponseContent = null)
        {
            _jsonSerializerSettings = jsonSerializerSettings ??
                new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };
            if (invalidResponseContent?.Count == 0)
            {
                _invalidResponseContent = new List<string>
                {
                    "{}", "[]", "[{}]", "true", "false"
                };
            }
            _dataServiceGateway = new DataServiceGateway(
                baseUrl, apiSettings, apiFormatConverter);
        }

        public string BaseUrl => _dataServiceGateway.BaseUrl;

        public ApiSettings ApiSettings => _dataServiceGateway
            .ApiSettings;

        public async Task<bool> ValueAsync(
            IAuthUser user,
            string uri,
            HttpMethods httpMethod)
        {
            var responseContent = await _dataServiceGateway
                .DataAsync(user, uri, httpMethod);

            bool.TryParse(responseContent, out bool value);
            return value;
        }

        public async Task<object> ValueAsync(IAuthUser user, string uri)
        {
            var responseContent = await _dataServiceGateway
                .DataAsync(user, uri, HttpMethods.Get);
            return responseContent;
        }

        public Task<IResponseFileStream> GetFileAsync(
            IAuthUser user, string uri)
        {
            var value = _dataServiceGateway.GetFileAsync(user, uri);
            return value;
        }

        public async Task<IEnumerable<T>> GetIndexAsync<T>(
            IAuthUser user, 
            string uri, 
            IDictionary<string, object> data = null)
            where T : IDataModel
        {
            var responseContent = await _dataServiceGateway
                .GetIndexAsync(user, uri, data);

            var models = ConvertIndex<T>(responseContent);
            return models;
        }

        public async Task<T> GetObjectAsync<T>(
            IAuthUser user, string uri)
            where T : IDataModel
        {
            var responseContent = await _dataServiceGateway
                .GetObjectAsync(user, uri);

            var value = Convert<T>(responseContent);
            return value;
        }


        public async Task<T> ScalarDataAsync<T>(
            IAuthUser user,
            string uri,
            HttpMethods httpMethod,
            IDictionary<string, object> data = null)
            where T : IDataModel
        {
            var responseContent = await _dataServiceGateway
                .DataAsync(user, uri, httpMethod, data);

            var value = Convert<T>(responseContent);
            return value;
        }

        public async Task<IEnumerable<T>> IndexDataAsync<T>(
            IAuthUser user,
            string uri,
            HttpMethods httpMethod,
            IDictionary<string, object> data = null)
            where T : IDataModel
        {
            var responseContent = await _dataServiceGateway
                .DataAsync(user, uri, httpMethod, data);

            var models = ConvertIndex<T>(responseContent);
            return models;
        }

        private IEnumerable<T> ConvertIndex<T>(string responseContent)
            where T : IDataModel
        {
            var models = Enumerable.Empty<T>();
            if (!Invalid(responseContent))
            {
                models = JsonConvert.DeserializeObject<IEnumerable<T>>(
                    responseContent, _jsonSerializerSettings);
            }
            return models;
        }

        private T Convert<T>(string responseContent)
            where T : IDataModel
        {
            var value = default(T);
            if (!Invalid(responseContent))
            {
                value = JsonConvert.DeserializeObject<T>(
                        responseContent, _jsonSerializerSettings);
            }
            return value;
        }

        private bool Invalid(string responseContent)
        {
            var value = _invalidResponseContent
                .Contains(responseContent.ToLower());
            return value;
        }
    }
}
