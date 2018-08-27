using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SixNations.Desktop.Interfaces
{
    public interface IDataService<T> where T : IDataServiceModel
    {
        

        Task<IEnumerable<T>> GetModelDataAsync(
            string authToken, Action<Exception> exceptionHandler);
    }
}