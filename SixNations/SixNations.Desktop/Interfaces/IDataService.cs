using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SixNations.Desktop.Interfaces
{
    public interface IDataService<T> where T : IDataServiceModel
    {
        Task<T> CreateModelAsync(string authToken, Action<Exception> exceptionHandler);

        Task<T> StoreModelAsync(string authToken, Action<Exception> exceptionHandler, T model);

        Task<T> EditModelAsync(string authToken, Action<Exception> exceptionHandler, T model);

        Task<T> EditModelAsync(string authToken, Action<Exception> exceptionHandler, int modelId);

        Task<T> UpdateModelAsync(string authToken, Action<Exception> exceptionHandler, T model);

        Task<bool> DeleteModelAsync(string authToken, Action<Exception> exceptionHandler, T model);

        Task<IEnumerable<T>> GetModelDataAsync(string authToken, Action<Exception> exceptionHandler);
    }
}