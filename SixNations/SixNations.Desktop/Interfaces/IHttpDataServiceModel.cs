using SixNations.Desktop.Models;
using System.Collections.Generic;

namespace SixNations.Desktop.Interfaces
{
    public interface IHttpDataServiceModel : IDataServiceModel
    {
        void Initialise(DataTransferObject dto);

        string Error { get; set; }

        bool IsLockedForEditing { get; }

        IDictionary<string, object> Data { get; }
    }
}
