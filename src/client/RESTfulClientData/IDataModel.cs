using System.Collections.Generic;

namespace savaged.mvvm.Data
{
    public interface IDataModel
    {
        int Id { get; set; }

        string Name { get; set; }

        bool IsNew { get; }
    }
}