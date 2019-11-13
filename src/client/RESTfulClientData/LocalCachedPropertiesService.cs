using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace savaged.mvvm.Data
{
    public class LocalCachedPropertiesService 
        : ILocalCachedPropertiesService
    {
        private readonly string _fileLocation;

        private readonly IDictionary<string, object> _props;

        public LocalCachedPropertiesService(
            string localCachedPropertiesFileLocation)
        {
            _fileLocation = localCachedPropertiesFileLocation;

            var fileInfo = new FileInfo(_fileLocation);
            if (fileInfo.Exists)
            {
                _props = JsonConvert
                    .DeserializeObject<IDictionary<string, object>>(
                    File.ReadAllText(_fileLocation));
            }
            else
            {
                _props = new Dictionary<string, object>();
            }
        }

        public object Get(string key)
        {
            if (_props.Keys.Contains(key))
            {
                return _props[key];
            }
            return null;
        }

        public void Set(string key, object value)
        {
            if (_props.Keys.Contains(key))
            {
                _props[key] = value;
            }
            else
            {
                _props.Add(key, value);
            }

            var fileInfo = new FileInfo(_fileLocation);
            if (fileInfo.Exists)
            {
                File.Delete(_fileLocation);
            }
            var json = JsonConvert.SerializeObject(_props);

            File.WriteAllText(_fileLocation, json);
        }
    }
}
