using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using SixNations.Desktop.Interfaces;
using log4net;

namespace SixNations.Desktop.Models
{
    public class ResponseRootObject : IResponseRootObject
    {
        private IDataTransferObject[] _data;
        private string _error;

        public ResponseRootObject(string error) : this()
        {
            Success = false;
            _error = error;
            Data.First().AddError(error);
        }

        [JsonConstructor]
        public ResponseRootObject(IDataTransferObject[] data) : this()
        {
            if (data != null && data.Length > 0)
            {
                _data = data;
            }
        }

        public ResponseRootObject()
        {
            _data = new DataTransferObject[] { new DataTransferObject() };
        }

        public IDataTransferObject[] Data
        {
            get => _data;
            set
            {
                if (value != null && value.Length > 0)
                {
                    _data = value;
                }
            }
        }

        [JsonProperty(PropertyName = "error")]
        public string Error
        {
            get => _error;
            set
            {
                if (value != null)
                {
                    _error = value;
                }
            }
        }

        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }

        /// <summary>
        /// Decorating API which doesn't expose this.
        /// </summary>
        public void __SetIsLockedForEditing()
        {
            foreach (var rdo in Data)
            {
                rdo.__SetIsLockedForEditing();
            }
        }

        public bool IsEmpty()
        {
            var isEmptyCount = 0;
            foreach (var rdo in Data)
            {
                isEmptyCount = rdo.IsEmpty() ? 0 : 1;
            }
            return isEmptyCount == 0;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    [JsonObject]
    public class DataTransferObject : IDataTransferObject
    {
        public DataTransferObject()
        {
            Fields = new Dictionary<string, object>();
            Raw = new Dictionary<string, JToken>();
        }

        [JsonExtensionData]
        private IDictionary<string, JToken> Raw { get; set; }

        public object this[string key] => Fields[key];

        private IDictionary<string, object> Fields { get; }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            foreach (KeyValuePair<string, JToken> kvp in Raw)
            {
                var jToken = kvp.Value;
                dynamic value;
                switch (jToken.Type)
                {
                    case JTokenType.Integer:
                        value = jToken.ToObject<int>();
                        break;
                    case JTokenType.String:
                        var raw = jToken.ToObject<string>();
                        if (raw == string.Empty)
                        {
                            value = null;
                            break;
                        }
                        DateTime date;
                        var @try = DateTime.TryParse(raw, out date);
                        if (@try || raw == "0000-00-00")
                        {
                            value = date;
                            break;
                        }
                        var isTelephoneNumber = false;
                        if (raw.TrimStart().StartsWith("0") && !raw.StartsWith("0.") || raw.TrimStart().StartsWith("+"))
                        {
                            isTelephoneNumber = true;
                        }
                        if (!isTelephoneNumber && raw.Contains("."))
                        {
                            @try = decimal.TryParse(raw, out decimal dec);
                            if (@try)
                            {
                                value = dec;
                                break;
                            }
                        }
                        if (!isTelephoneNumber)
                        {
                            @try = int.TryParse(raw, out int @int);
                            if (@try)
                            {
                                value = @int;
                                break;
                            }
                        }
                        value = raw;
                        break;
                    case JTokenType.Boolean:
                        value = jToken.ToObject<bool>();
                        break;
                    case JTokenType.Date:
                        value = jToken.ToObject<DateTime>();
                        break;
                    case JTokenType.Float:
                        value = jToken.ToObject<decimal>();
                        break;
                    default:
                        value = null;
                        break;
                }
                Fields.Add(kvp.Key, value);
            }
        }

        /// <summary>
        /// Decorating API which doesn't expose this.
        /// </summary>
        public void __SetIsLockedForEditing()
        {
            var key = "IsLockedForEditing";
            if (!Fields.ContainsKey(key))
            {
                Fields.Add(key, true);
            }
        }

        public void AddError(string error)
        {
            var key = "Error";
            if (!Fields.ContainsKey(key))
            {
                Fields.Add(key, error);
            }
        }

        public bool IsEmpty()
        {
            return Fields == null || Fields.Count == 0;
        }

        public bool ContainsKey(string key)
        {
            return Fields.ContainsKey(key);
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class ResponseConverter : CustomCreationConverter<IResponseRootObject>
    {
        public override IResponseRootObject Create(Type T)
        {
            return new ResponseRootObject();
        }
    }

    public class ResponseRootObjectToModelMapper<T> where T : IHttpDataServiceModel, new()
    {
        private static readonly ILog Log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private bool _noData;
        private IResponseRootObject _responseRootObject;

        public ResponseRootObjectToModelMapper(IResponseRootObject responseRootObject)
        {
            _responseRootObject = responseRootObject;
        }

        public T Mapped()
        {
            Validate();
            return Map();
        }

        public IList<T> AllMapped()
        {
            Validate();
            return MapAll().ConvertAll(o => o);
        }

        private void Validate()
        {
            if (_responseRootObject == null)
            {
                throw new ArgumentNullException(
                    "No response root found. " +
                    "The service relied upon should always return a response root object.");
            }
            if (_responseRootObject.IsEmpty())
            {
                _noData = true;
            }
        }

        private T Map()
        {
            var model = new T();
            if (!_noData)
            {
                if (_responseRootObject.Data.Length > 1)
                {
                    throw new InvalidOperationException(
                        "Multiple data items found. NOTE to developer: Use MapAll() instead.");
                }
                try
                {
                    model = MapAll().First();
                }
                catch (ArgumentException ex)
                {
                    Log.Error($"Unexpected error mapping response to model! {ex.Message}");
                    throw;
                }
            }
            return model;
        }

        private List<T> MapAll()
        {
            var models = new List<T>();
            foreach (var rdo in _responseRootObject.Data)
            {
                if (!rdo.IsEmpty())
                {
                    try
                    {
                        var model = new T();
                        model.Initialise(rdo);
                        models.Add(model);
                    }
                    catch (ArgumentException ex)
                    {
                        Log.Error($"Unexpected error mapping response to models! {ex.Message}");
                        throw;
                    }
                }
            }
            return models;
        }
    }
}
