using System.Collections.Generic;
using System.Threading.Tasks;

namespace savaged.mvvm.Data
{
    public class ModelService : IModelService
    {
        private readonly IModelServiceGateway _modelServiceGateway;
        private readonly UriActionBuilder _uriActionBuilder;

        public ModelService(
            IModelServiceGateway modelServiceGateway)
        {
            _modelServiceGateway = modelServiceGateway;
            _uriActionBuilder = new UriActionBuilder(
                _modelServiceGateway
                .ApiSettings?.IsIndexExplicitlyPlural);
        }

        public string BaseUrl => _modelServiceGateway?.BaseUrl;

        public string ExpectedApiVersionNumber => 
            _modelServiceGateway?.ApiSettings?.ExpectedApiVersion;

        public async Task<IEnumerable<T>> SearchAsync<T>(
            IAuthUser user, IDictionary<string, object> data)
            where T : IDataModel
        {
            var uri = _uriActionBuilder.BuildSearch(typeof(T));
            var index = await _modelServiceGateway.IndexDataAsync<T>(
                user, uri, HttpMethods.Post, data);
            return index;
        }

        public async Task<IEnumerable<T>> IndexAsync<T>(
            IAuthUser user,
            IDataModel relative = null,
            IDictionary<string, object> args = null)
            where T : IDataModel
        {
            var uri = _uriActionBuilder.BuildIndex(typeof(T), relative);

            var index = await IndexAsync<T>(user, uri, args);
            return index;
        }
        public async Task<IEnumerable<T>> IndexAsync<T>(
            IAuthUser user,
            IList<IDataModel> related,
            IDictionary<string, object> args = null)
            where T : IDataModel
        {
            var uri = _uriActionBuilder.BuildIndex(typeof(T), related);

            var index = await IndexAsync<T>(user, uri, args);
            return index;
        }
        private async Task<IEnumerable<T>> IndexAsync<T>(
            IAuthUser user, 
            string uri, 
            IDictionary<string, object> args = null)
            where T : IDataModel
        {
            var index = await _modelServiceGateway.GetIndexAsync<T>(
                user, uri, args);
            return index;
        }

        public async Task<T> CreateAsync<T>(
            IAuthUser user, IDataModel relative)
            where T : IDataModel, new()
        {
            var uri = _uriActionBuilder.BuildCreate(typeof(T), relative);

            T value = await CreateAsync<T>(user, uri);
            return value;
        }
        public async Task<T> CreateAsync<T>(
            IAuthUser user, IList<IDataModel> related)
            where T : IDataModel, new()
        {
            var uri = _uriActionBuilder.BuildCreate(
                typeof(T), related);

            T value = await CreateAsync<T>(user, uri);
            return value;
        }
        private async Task<T> CreateAsync<T>(IAuthUser user, string uri)
            where T : IDataModel, new()
        {
            T value = await _modelServiceGateway.GetObjectAsync<T>(user, uri);
            if (value == null)
            {
                value = new T();
            }
            return value;
        }

        public async Task<T> EditAsync<T>(IAuthUser user, T model)
            where T : IDataModel
        {
            if (model.Equals(default(T))) return model;

            var uri = _uriActionBuilder.BuildEdit(model);

            var value = await _modelServiceGateway.GetObjectAsync<T>(
                user, uri);
            return value;
        }

        public async Task<T> ShowAsync<T>(
            IAuthUser user, 
            T model,
            IDictionary<string, object> args = null)
            where T : IDataModel
        {
            if (model.Equals(default(T))) return model;

            var uri = _uriActionBuilder.BuildShow(model);

            var value = await _modelServiceGateway.GetObjectAsync<T>(user, uri);
            return value;
        }
        public async Task<T> ShowAsync<T>(
            IAuthUser user,
            int modelId,
            IDictionary<string, object> args = null)
            where T : IDataModel
        {
            if (modelId < 1) return default;

            var uri = _uriActionBuilder.BuildShow(typeof(T).Name, modelId);

            var value = await _modelServiceGateway.GetObjectAsync<T>(user, uri);
            return value;
        }

        public async Task<T> ShowAsync<T>(
            IAuthUser user,
            IDataModel relative,
            IDictionary<string, object> args = null)
            where T : IDataModel
        {
            var uri = _uriActionBuilder.BuildShow(typeof(T), relative, args);

            var value = await _modelServiceGateway.GetObjectAsync<T>(user, uri);
            return value;
        }
        public async Task<T> ShowAsync<T>(
            IAuthUser user,
            IList<IDataModel> related)
            where T : IDataModel
        {
            var uri = _uriActionBuilder.BuildShow(related);

            var value = await _modelServiceGateway.GetObjectAsync<T>(user, uri);
            return value;
        }

        public async Task<V> ShowFieldValueAsync<T, V>(
            IAuthUser user,
            T model,
            string fieldName)
            where T : IDataModel
        {
            var obj = await ShowFieldValueAsync(user, model, fieldName);
            if (obj is V value)
            {
                return value;
            }
            else
            {
                value = obj.ChangeType<V>();
                return value;
            }
        }
        private async Task<object> ShowFieldValueAsync<T>(
            IAuthUser user,
            T model,
            string fieldName)
            where T : IDataModel
        {
            var uri = $"fieldvalue/{model?.GetType()?.Name?.ToUriFormat()}/" +
                $"{model?.Id}/{fieldName?.ToUriFormat()}";

            var value = await _modelServiceGateway.ValueAsync(user, uri);
            return value;
        }


        public async Task<T> StoreAsync<T>(
            IAuthUser user, 
            T model, 
            IDataModel relative = null,
            IDictionary<string, object> args = null)
            where T : IDataModel
        {
            if (model.Equals(default(T))) return model;

            var data = model.ToDictionary();

            var uri = _uriActionBuilder.BuildStore(model, relative, args);

            var value = await StoreAsync<T>(user, uri, data);
            return value;
        }
        public async Task<T> StoreAsync<T>(
            IAuthUser user,
            T model,
            IList<IDataModel> related,
            IDictionary<string, object> args = null)
            where T : IDataModel
        {
            if (model.Equals(default(T))) return model;

            var data = model.ToDictionary();

            var uri = _uriActionBuilder.BuildStore(model, related, args);

            var value = await StoreAsync<T>(user, uri, data);
            return value;
        }
        private async Task<T> StoreAsync<T>(
            IAuthUser user, string uri, IDictionary<string, object> data)
            where T : IDataModel
        {
            var value = await _modelServiceGateway.ScalarDataAsync<T>(
                user, uri, HttpMethods.Post, data);
            return value;
        }


        public async Task<T> UpdateAsync<T>(IAuthUser user, T model)
            where T : IDataModel
        {
            if (model.Equals(default(T))) return model;

            var uri = _uriActionBuilder.BuildUpdate(model);

            var value = await UpdateAsync(user, model, uri);
            return value;
        }

        private async Task<T> UpdateAsync<T>(
            IAuthUser user,
            T model,
            string uri)
            where T : IDataModel
        {
            var data = model.ToDictionary();
            var value = await _modelServiceGateway.ScalarDataAsync<T>(
                user, uri, HttpMethods.Put, data);
            return value;
        }


        public async Task DeleteAsync<T>(IAuthUser user, T model)
            where T : IDataModel
        {
            if (model.Equals(default(T))) return;

            var uri = _uriActionBuilder.BuildDestroy(model);

            await _modelServiceGateway.ValueAsync(
                user, uri, HttpMethods.Delete);
        }

        public async Task ArchiveAsync<T>(IAuthUser user, T model)
            where T : IDataModel
        {
            if (model.Equals(default(T))) return;

            var uri = _uriActionBuilder.BuildArchive(model);

            await _modelServiceGateway.ValueAsync(
                user, uri, HttpMethods.Delete);
        }

        public async Task UnlockAsync<T>(IAuthUser user, T model)
            where T : IDataModel
        {
            if (model.Equals(default(T))) return;

            var uri = _uriActionBuilder.BuildUnlock(model);

            await _modelServiceGateway.ValueAsync(
                user, uri, HttpMethods.Post);
        }
    }
}
