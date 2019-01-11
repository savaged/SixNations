using System.Collections;
using System.Collections.Generic;

namespace SixNations.Server.Models
{
    public class ResponseRootObject
    {
        public ResponseRootObject()
        {
            Data = new ArrayList();
        }

        public ResponseRootObject(ModelBase model)
            : this()
        {
            SetData(model);
        }

        public ResponseRootObject(IEnumerable<ModelBase> models)
            : this()
        {
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

        private void SetData(ModelBase model)
        {
            Data.Add(model);
        }
    }
}
