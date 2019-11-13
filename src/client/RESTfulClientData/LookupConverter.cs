using Newtonsoft.Json;
using System.Collections.Generic;

namespace savaged.mvvm.Data
{
    internal class LookupConverter
    {
        private readonly IAuthUser _user;

        public LookupConverter(IAuthUser user)
        {
            _user = user;
        }

        public ILookup Convert(string responseContent)
        {
            var lookup = new Lookup();
            if (IsNullOrEmpty(responseContent))
            {
                return lookup;
            }
            LookupDataTransferObject dto = null;
            try
            {
                dto = JsonConvert
                    .DeserializeObject<LookupDataTransferObject>(
                        responseContent);
            }
            catch (JsonSerializationException ex)
            {
                throw new LookupConversionException(
                    responseContent, ex, _user);
            }
            foreach (var kvp in dto.Data)
            {
                lookup.Add(kvp);
            }
            return lookup;
        }

        public ILookup Convert<T>(string responseContent)
            where T : IDataModel
        {
            var lookup = new Lookup();
            if (IsNullOrEmpty(responseContent))
            {
                return lookup;
            }
            IEnumerable<T> models;
            try
            {
                models = JsonConvert
                    .DeserializeObject<IEnumerable<T>>(responseContent);
            }
            catch (JsonSerializationException ex)
            {
                throw new LookupConversionException(
                    responseContent, ex, _user);
            }
            foreach (var model in models)
            {
                lookup.Add(model.Id, model.Name);
            }
            return lookup;
        }

        private bool IsNullOrEmpty(string responseContent)
        {
            var value = string.IsNullOrEmpty(responseContent)
                || responseContent == "[]"
                || LookupDataTransferObject.IsNullOrEmpty(responseContent);
            return value;
        }
    }
}
