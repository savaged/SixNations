using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommonServiceLocator;
using SixNations.Desktop.Constants;
using SixNations.Desktop.Helpers;
using SixNations.Desktop.Interfaces;
using SixNations.Desktop.Models;

namespace SixNations.Desktop.Services
{
    public class RequirementDataService : IHttpDataService<Requirement>
    {
        public async Task<Requirement> CreateModelAsync(
            string authToken, Action<Exception> exceptionHandler)
        {
            var uri = $"{typeof(Requirement).NameToUriFormat()}/create";
            var response = await ServiceLocator.Current.GetInstance<IHttpDataServiceFacade>()
                .HttpRequestAsync(uri, User.Current.AuthToken);
            var model = new ResponseRootObjectToModelMapper<Requirement>(response).Mapped();
            return model;
        }

        public async Task<Requirement> StoreModelAsync(
            string authToken, Action<Exception> exceptionHandler, Requirement model)
        {
            if (!model.IsNew)
            {
                throw new ArgumentException("Trying to store an existing model!", nameof(model));
            }
            var uri = typeof(Requirement).NameToUriFormat();
            var data = model.GetData();
            var response = await ServiceLocator.Current.GetInstance<IHttpDataServiceFacade>()
                .HttpRequestAsync(uri, User.Current.AuthToken, HttpMethods.Post, data);
            model = new ResponseRootObjectToModelMapper<Requirement>(response).Mapped();
            return model;
        }

        public async Task<IEnumerable<Requirement>> GetModelDataAsync(
            string authToken, Action<Exception> exceptionHandler)
        {
            var uri = typeof(Requirement).NameToUriFormat();

            ResponseRootObject response = null;
            try
            {
                response = await ServiceLocator.Current.GetInstance<IHttpDataServiceFacade>()
                    .HttpRequestAsync(uri, authToken);
            }
            catch (Exception ex)
            {
                exceptionHandler(ex);
                response = new ResponseRootObject(ex.Message);
            }
            var index = new ResponseRootObjectToModelMapper<Requirement>(response).AllMapped();
            return index;
        }

        public async Task<Requirement> EditModelAsync(
            string authToken, Action<Exception> exceptionHandler, int modelId)
        {
            var isNew = modelId < 1;
            if (isNew)
            {
                throw new NotSupportedException("Trying to edit a new model! Use the store method instead.");
            }
            var uri = $"{typeof(Requirement).NameToUriFormat()}/{modelId}/edit";
            var response = await ServiceLocator.Current.GetInstance<IHttpDataServiceFacade>()
                    .HttpRequestAsync(uri, authToken);
            var model = new ResponseRootObjectToModelMapper<Requirement>(response).Mapped();
            return model;
        }

        public async Task<Requirement> EditModelAsync(
            string authToken, Action<Exception> exceptionHandler, Requirement model)
        {
            model = await EditModelAsync(authToken, exceptionHandler, model.Id);
            return model;
        }

        public async Task<Requirement> UpdateModelAsync(
            string authToken, Action<Exception> exceptionHandler, Requirement model)
        {
            if (model.IsNew)
            {
                throw new ArgumentException("Trying to update a new model!", nameof(model));
            }
            var uri = $"{typeof(Requirement).NameToUriFormat()}/{model.Id}";
            var data = model.GetData();
            var response = await ServiceLocator.Current.GetInstance<IHttpDataServiceFacade>()
                .HttpRequestAsync(uri, User.Current.AuthToken, HttpMethods.Put, data);
            model = new ResponseRootObjectToModelMapper<Requirement>(response).Mapped();
            return model;
        }

        public async Task<bool> DeleteModelAsync(
            string authToken, Action<Exception> exceptionHandler, Requirement model)
        {
            if (model.IsNew)
            {
                throw new ArgumentException("Trying to delete a new model!", nameof(model));
            }
            var uri = $"{typeof(Requirement).NameToUriFormat()}/{model.Id}";
            var data = model.GetData();
            var response = await ServiceLocator.Current.GetInstance<IHttpDataServiceFacade>()
                .HttpRequestAsync(uri, User.Current.AuthToken, HttpMethods.Delete, null);
            return response.Success;
        }
    }
}
