using Newtonsoft.Json;
using System.Collections.Generic;

namespace savaged.mvvm.Data
{
    class LookupDataTransferObject 
    {
        private string _type;

        public LookupDataTransferObject()
        {
            Data = new List<KeyValuePair<int, string>>();
        }

        public IEnumerable<KeyValuePair<int, string>> Data
        { get; set; }

        public string Type
        {
            get => _type;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _type = "Missing";
                }
                else
                {
                    if (value.Contains(" ") || value.Contains("\\"))
                    {
                        _type = "Malformed";
                    }
                    else
                    {
                        _type = value;
                    }
                }
            }
        }

        public override string ToString()
        {
            var value = JsonConvert.SerializeObject(this);
            return value;
        }

        public static bool IsNullOrEmpty(string rawLookupDataTransfer)
        {
            var value = !string.IsNullOrEmpty(rawLookupDataTransfer);
            value &= rawLookupDataTransfer.Length <= AsString().Length;
            return value;
        }

        public static string AsString()
        {
            var example = new LookupDataTransferObject
            {
                Type = string.Empty
            };
            var exampleData = new List<KeyValuePair<int, string>>
            {
                new KeyValuePair<int, string>(0, string.Empty)
            };
            example.Data = exampleData;
            return example.ToString();
        }

        public static string AsExampleString()
        {
            var example = new LookupDataTransferObject
            {
                Type = "LookupName"
            };
            var exampleData = new List<KeyValuePair<int, string>>
            {
                new KeyValuePair<int, string>(0, "Lookup Value")
            };
            example.Data = exampleData;
            return example.ToString();
        }
    }
}
