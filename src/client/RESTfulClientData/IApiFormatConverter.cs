using System.Collections.Generic;

namespace savaged.mvvm.Data
{
    public interface IApiFormatConverter
    {
        IDictionary<string, object> Convert(
            IDictionary<string, object> data);
    }
}
