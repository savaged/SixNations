using System;
using System.IO;
using System.Collections.Generic;

namespace SixNations.Desktop.Interfaces
{
    public interface IIndexExcelAdapter<T> : IDisposable
        where T : IHttpDataServiceModel
    {
        bool CanExecute { get; }

        void Adapt(IList<T> index);

        IList<T> Adapt(FileInfo fi);
    }
}