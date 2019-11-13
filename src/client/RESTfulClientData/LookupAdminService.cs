using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace savaged.mvvm.Data
{
    public class LookupAdminService : ILookupAdminService
    {
        private const string _uriPrefix = "typedlookup";
        private readonly IDataServiceGateway _dataServiceGateway;
        private readonly IStaticLookupService _lookupService;
        private readonly UriActionBuilder _uriActionBuilder;

        public LookupAdminService(
            string baseUrl,
            IStaticLookupService lookupService,
            ApiSettings apiSettings)
        {
            _dataServiceGateway = new DataServiceGateway(
                baseUrl, apiSettings);

            _uriActionBuilder = new UriActionBuilder();

            _lookupService = lookupService ??
                throw new ArgumentNullException(nameof(lookupService));
        }

        public async Task<bool> CanAdd(IAuthUser user, string lookupName)
        {
            bool result;
            try
            {
                var uri = _uriActionBuilder.BuildCreate(
                    _uriPrefix + lookupName);

                var response = await GetDataServiceResponse(user, uri);

                result = !string.IsNullOrEmpty(response);

                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public async Task<bool> CanEdit(
            IAuthUser user, string lookupName, int key)
        {
            bool result;
            try
            {
                var uri = _uriActionBuilder.BuildEdit(
                    _uriPrefix + lookupName, key);

                var response = await GetDataServiceResponse(user, uri);

                result = !string.IsNullOrEmpty(response);
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public async Task Store(IAuthUser user, string lookupName, string value)
        {
            var uri = _uriActionBuilder.BuildStore(
                _uriPrefix + lookupName);

            var data = GetData(lookupName, value);

            await _dataServiceGateway.DataAsync(
                user,
                uri,
                HttpMethods.Post,
                data);
        }

        public async Task Update(
            IAuthUser user, string lookupName, int key, string value)
        {
            var uri = _uriActionBuilder.BuildUpdate(
                _uriPrefix + lookupName, key);

            var data = GetData(lookupName, value);

            await _dataServiceGateway.DataAsync(
                user,
                uri,
                HttpMethods.Post,
                data);
        }

        private IDictionary<string, object> GetData(
            string lookupName, string value)
        {
            var data = new Dictionary<string, object>
            {
                { GetLookupDataKey(lookupName), value }
            };
            return data;
        }

        private string GetLookupDataKey(string lookupName)
        {
            return $"{lookupName}Name";
        }

        private async Task<string> GetDataServiceResponse(
            IAuthUser user, string uri)
        {
            var response = await _dataServiceGateway.DataAsync(
                user, uri, HttpMethods.Get);
            return response;
        }
    }
}


