using System.Collections.Generic;
using System.Threading.Tasks;

namespace SixNations.Desktop.Interfaces
{
    public interface IIndexToExcelAdapter<T>
        where T : IHttpDataServiceModel
    {
        bool CanExecute { get; }

        void Adapt(IList<T> index);
    }
}