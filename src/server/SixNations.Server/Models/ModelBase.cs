using System.Collections.Generic;

namespace SixNations.Server.Models
{
    public abstract class ModelBase
    {
        public IDictionary<string, object> GetData()
        {
            var data = new Dictionary<string, object>();
            var props = GetType().GetProperties();
            foreach (var p in props)
            {
                var value = p.GetValue(this);
                data.Add(p.Name, value);
            }
            return data;
        }
    }
}
