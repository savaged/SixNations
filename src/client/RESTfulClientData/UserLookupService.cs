using System.Collections.Generic;
using System.Threading.Tasks;

namespace savaged.mvvm.Data
{
    public class UserLookupService : IUserLookupService
    {
        private readonly IDataServiceGateway _dataServiceGateway;

        public UserLookupService(
            string baseUrl,
            ApiSettings apiSettings)
        {
            _dataServiceGateway = new DataServiceGateway(
                baseUrl, apiSettings);
        }

        public async Task<ILookup> GetAsync(
            IAuthUser user, 
            IDataModel relation, 
            string relationUserField)
        {
            var args = (new Dictionary<string, object>
            {
                { "field", relationUserField }
            }).ToUriParams();

            var uri = 
                $"usersby{relation?.GetType()?.Name?.ToUriFormat()}/" +
                $"{relation?.Id}{args}";           

            var responseContent = await _dataServiceGateway
                .GetObjectAsync(user, uri);

            var value = new LookupConverter(user)
                .Convert(responseContent);
            return value;
        }

        public async Task<ILookup> GetAsync(IAuthUser user)
        {
            var responseContent = await _dataServiceGateway
                .GetObjectAsync(user, "users");

            var value = new LookupConverter(user)
                .Convert(responseContent);
            return value;
        }
    }
}


