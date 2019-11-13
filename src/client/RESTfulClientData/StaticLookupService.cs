using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace savaged.mvvm.Data
{
    public class StaticLookupService : IStaticLookupService
    {
        private readonly IDataServiceGateway _dataServiceGateway;

        public StaticLookupService(
            string baseUrl,
            ApiSettings apiSettings)
        {
            Index = new Dictionary<string, ILookup>();

            _dataServiceGateway = new DataServiceGateway(
                baseUrl, apiSettings);
        }

        public IDictionary<string, ILookup> Index { get; }

        public bool IsLoaded => Index.Count > 0;

        public async Task LoadAsync(IAuthUser user)
        {
            if (Index.Count > 0)
            {
                // Static so should only be loaded once
                return;
            }
            var responseContent = await _dataServiceGateway
                .GetIndexAsync(user, "typedlookup");

            var dtos = JsonConvert
                .DeserializeObject<IEnumerable<LookupDataTransferObject>>(
                responseContent);

            Index.Clear();

            foreach (var dto in dtos)
            {
                var dict = new Lookup();
                foreach (var kvp in dto.Data)
                {
                    dict.Add(kvp);
                }
                Index.Add(dto.Type, dict);
            }
        }

        public ILookup Get(string lookupName)
        {
            ILookup value = new Lookup();

            if (Index.ContainsKey(lookupName))
            {
                value = Index[lookupName];
            }
            return value;
        }

    }
}


