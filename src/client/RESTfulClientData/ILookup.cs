using System.Collections.Generic;

namespace savaged.mvvm.Data
{
    public interface ILookup : IDictionary<int, string>
    {
        int GetKeyFromValue(string value);

        ILookup ToOrdered(bool descending = false);
    }
}