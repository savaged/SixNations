using savaged.mvvm.Core.Interfaces;
using savaged.mvvm.Data;
using savaged.mvvm.ViewModels.Core.Utils;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Threading.Tasks;

namespace savaged.mvvm.ViewModels.Core
{
    public abstract class BaseLookupsComponent<T> : ObservableObject
        where T : IObservableModel, new()
    {
        private readonly ExceptionHandlingService _exceptionHandlingService;
        private readonly IViewModelCommonParams _commonParams;
        private T _modelObject;

        public BaseLookupsComponent(
            IViewModelCommonParams commonParams,
            IMessenger messengerInstance,
            T modelObject = default)
        {
            _exceptionHandlingService = new ExceptionHandlingService();
            MessengerInstance = messengerInstance ??
                throw new ArgumentNullException(nameof(messengerInstance));
            _commonParams = commonParams ??
                throw new ArgumentNullException(nameof(commonParams));

            ModelObject = modelObject?.Equals(default) == false ?
                modelObject : new T();
        }

        public T ModelObject
        {
            get => _modelObject;
            private set => Set(ref _modelObject, value);
        }

        public abstract Task<bool> LoadAsync();

        protected ILookupServiceLocator GetLookupServiceLocator()
        {
            var value = _commonParams?.LookupServiceLocator ??
                    throw new ArgumentNullException(
                        nameof(IViewModelCommonParams.LookupServiceLocator));
            return value;
        }

        protected IMessenger MessengerInstance { get; }

        protected virtual void ReactToException(
            object origin, DesktopException ex)
        {
            _exceptionHandlingService.ReactToException(origin, ex);
        }

    }
}
