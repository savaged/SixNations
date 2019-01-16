using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SixNations.Server.Models
{
    public class ResponseRootObject
    {
        private static int[] _successCodes =
        {
            200,
            201,
            202
        };

        public ResponseRootObject(int statusCode)
        {
            Success = _successCodes.Contains(statusCode);
            Data = new ArrayList();
        }

        public ResponseRootObject(int statusCode, ModelBase model)
            : this(statusCode)
        {
            SetData(model);
        }

        public ResponseRootObject(int statusCode, IEnumerable<ModelBase> models)
            : this(statusCode)
        {
            SetData(models);
        }

        public ResponseRootObject(int statusCode, IDictionary<string, object> data)
            : this(statusCode)
        {
            SetData(data);
        }

        public bool Success { get; set; }

        public string Error { get; set; }

        public IList Data { get; private set; }

        private void SetData(IEnumerable<ModelBase> models)
        {
            foreach (var model in models)
            {
                Data.Add(model);
            }
        }

        private void SetData(ModelBase model)
        {
            Data.Add(model);
        }

        private void SetData(IDictionary<string, object> data)
        {
            foreach (var kvp in data)
            {
                Data.Add(new { kvp.Key, kvp.Value });
            }
        }
    }
}
