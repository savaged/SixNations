using System;

namespace SixNations.API.Interfaces
{
    public interface IHttpDataService<T> : IDataService<T> 
        where T : IHttpDataServiceModel
    {
    }
}