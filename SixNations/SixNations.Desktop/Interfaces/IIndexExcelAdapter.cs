using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SixNations.Desktop.Interfaces
{
    public interface IIndexExcelAdapter<T>
        where T : IHttpDataServiceModel
    {
        bool CanExecute { get; }

        Task AdaptAsync(IList<T> index);

        void Adapt(IList<T> index);

        Task<IList<T>> AdaptAsync(FileInfo fi);

        IList<T> Adapt(FileInfo fi);
    }
}