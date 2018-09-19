using System;

namespace SixNations.API.Interfaces
{
    public interface IDataServiceModel
    {
        bool IsDirty { get; }

        bool IsNew { get; }

        int Id { get; }

        string ToJson();
    }
}
