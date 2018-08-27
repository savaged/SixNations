using System;

namespace SixNations.Desktop.Interfaces
{
    public interface IHttpDataService<T> : IDataService<T> 
        where T : IHttpDataServiceModel
    {
    }
}