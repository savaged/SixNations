using SixNations.Desktop.Models;

namespace SixNations.Desktop.Interfaces
{
    public interface IHttpDataServiceModel : IDataServiceModel
    {
        void Initialise(DataTransferObject dto);

        string Error { get; set; }

        bool IsLockedForEditing { get; }
    }
}
