using System.Collections;
using System.Collections.Generic;

namespace SixNations.Server.Models
{
    public class ResponseRootObject
    {
        public ResponseRootObject(IEnumerable<ModelBase> models)
        {
            Data = new ArrayList();
            SetData(models);
        }

        public string Error { get; set; }

        public IList Data { get; private set; }

        private void SetData(IEnumerable<ModelBase> models)
        {
            foreach (var model in models)
            {
                Data.Add(model);
            }
        }
    }
}
