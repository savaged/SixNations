using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace savaged.mvvm.Data
{
    public interface IModelService 
    {
        string BaseUrl { get; }

        string ExpectedApiVersionNumber { get; }

        Task<T> CreateAsync<T>(
            IAuthUser user, IDataModel relative)
            where T : IDataModel, new();
        Task<T> CreateAsync<T>(
            IAuthUser user, IList<IDataModel> related)
            where T : IDataModel, new();

        Task<T> EditAsync<T>(IAuthUser user, T model)
            where T : IDataModel;

        Task ArchiveAsync<T>(IAuthUser user, T model)
            where T : IDataModel;

        Task DeleteAsync<T>(IAuthUser user, T model)
            where T : IDataModel;

        Task<IEnumerable<T>> IndexAsync<T>(
            IAuthUser user, 
            IDataModel relative = null, 
            IDictionary<string, object> args = null)
            where T : IDataModel;
        Task<IEnumerable<T>> IndexAsync<T>(
            IAuthUser user,
            IList<IDataModel> related,
            IDictionary<string, object> args = null)
            where T : IDataModel;

        Task<IEnumerable<T>> SearchAsync<T>(
            IAuthUser user, IDictionary<string, object> data)
            where T : IDataModel;


        Task<T> ShowAsync<T>(
            IAuthUser user, 
            T model, 
            IDictionary<string, object> args = null)
            where T : IDataModel;
        Task<T> ShowAsync<T>(
            IAuthUser user,
            int modelId,
            IDictionary<string, object> args = null)
            where T : IDataModel;
        Task<T> ShowAsync<T>(
            IAuthUser user,
            IDataModel relative,
            IDictionary<string, object> args = null)
            where T : IDataModel;
        Task<T> ShowAsync<T>(
            IAuthUser user,
            IList<IDataModel> related)
            where T : IDataModel;

        Task<V> ShowFieldValueAsync<T, V>(
            IAuthUser user,
            T model,
            string fieldName)
            where T : IDataModel;


        Task<T> StoreAsync<T>(
            IAuthUser user,
            T model, 
            IDataModel relative = null,
            IDictionary<string, object> args = null)
            where T : IDataModel;
        Task<T> StoreAsync<T>(
            IAuthUser user,
            T model,
            IList<IDataModel> related,
            IDictionary<string, object> args = null)
            where T : IDataModel;

        Task UnlockAsync<T>(IAuthUser user, T model)
            where T : IDataModel;

        Task<T> UpdateAsync<T>(IAuthUser user, T model)
            where T : IDataModel;
    }
}