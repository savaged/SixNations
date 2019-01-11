using SixNations.Server.Models;
using System.Collections.Generic;

namespace SixNations.Server.Controllers
{
    static class ModelToRootConverters
    {
        public static ResponseRootObject Convert<T>(
            IEnumerable<T> index) 
            where T : ModelBase
        {
            var root = new ResponseRootObject(index);
            return root;
        }
    }
}
