using System;
using System.Threading.Tasks;

namespace savaged.mvvm.Data
{
    public class DynamicLookupService : IDynamicLookupService
    {
        private readonly IDataServiceGateway _dataServiceGateway;

        public DynamicLookupService(
            string baseUrl,
            ApiSettings apiSettings)
        {
            _dataServiceGateway = new DataServiceGateway(
                baseUrl, apiSettings);
        }

        public async Task<ILookup> GetAsync<T>(IAuthUser user)
            where T : IDataModel
        {
            var lookup = await GetConvertedResponseAsync<T>(
                user, typeof(T).Name.ToUriFormat());
            return lookup;
        }
        public async Task<ILookup> GetAsync(
            IAuthUser user, string lookupType)
        {
            var lookup = await GetConvertedResponseAsync(
                user, lookupType.ToUriFormat());
            return lookup;
        }


        public async Task<ILookup> GetByRelationAsync(
            IAuthUser user, Type lookupType, IDataModel relation)
        {
            var uri = new UriActionBuilder().BuildIndex(lookupType, relation);
            var lookup = await GetConvertedResponseAsync(user, uri);
            return lookup;
        }

        public async Task<ILookup> GetByRelationAsync(
            IAuthUser user, string lookupName, IDataModel relation)
        {
            var uri = new UriActionBuilder().BuildIndex(lookupName, relation);
            var lookup = await GetConvertedResponseAsync(user, uri);
            return lookup;
        }

        public async Task<ILookup> GetByRelationAsync<T>(
            IAuthUser user,
            IDataModel relation)
            where T : IDataModel
        {
            if (relation == null)
            {
                throw new ArgumentNullException(nameof(relation));
            }
            var lookup = await GetByRelationAsync<T>(
                user, relation.GetType(), relation.Id);
            return lookup;
        }
        public async Task<ILookup> GetByRelationAsync<T>(
            IAuthUser user,
            Type relationType,
            int relationId)
            where T : IDataModel
        {
            var uri = new UriActionBuilder()
                .BuildIndex(typeof(T), relationType, relationId);
            var lookup = await GetConvertedResponseAsync<T>(user, uri);
            return lookup;
        }

        private async Task<ILookup> GetConvertedResponseAsync(
            IAuthUser user, string uri)
        {
            var responseContent = await _dataServiceGateway
                .GetObjectAsync(user, uri);
            var lookup = new LookupConverter(user)
                .Convert(responseContent);
            return lookup;
        }

        private async Task<ILookup> GetConvertedResponseAsync<T>(
            IAuthUser user, string uri)
            where T : IDataModel
        {
            var responseContent = await _dataServiceGateway
                .GetObjectAsync(user, uri);
            var lookupConverter = new LookupConverter(user);
            ILookup lookup;
            try
            {
                lookup = lookupConverter.Convert(responseContent);
            }
            catch (LookupConversionException)
            {
                try
                {
                    lookup = lookupConverter.Convert<T>(responseContent);
                }
                catch (ArgumentException ex)
                {
                    lookup = new Lookup(ex, 
                        "Is the lookup in the expected format? " +
                        LookupDataTransferObject.AsExampleString());
                }
            }
            return lookup;
        }

    }
}


