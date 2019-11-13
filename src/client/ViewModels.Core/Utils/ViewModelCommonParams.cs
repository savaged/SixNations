using savaged.mvvm.Core.Interfaces;
using savaged.mvvm.Data;
using savaged.mvvm.Navigation;

namespace savaged.mvvm.ViewModels.Core.Utils
{
    public class ViewModelCommonParams 
        : IViewModelCommonParams
    {
        public ViewModelCommonParams(
            IViewStateViewModel busyMonitor,
            INavigationService navigationService, 
            IModelService modelService,
            ILookupServiceLocator lookupServiceLocator,
            IFileService fileService,
            IFileHelperService fileHelper)
        {
            BusyMonitor = busyMonitor;
            NavigationService = navigationService;
            ModelService = modelService;
            LookupServiceLocator = lookupServiceLocator;
            FileService = fileService;
            FileHelper = fileHelper;
        }

        public IViewStateViewModel BusyMonitor { get; }

        public INavigationService NavigationService { get; }

        public IModelService ModelService { get; }

        public ILookupServiceLocator LookupServiceLocator { get; }

        public IFileService FileService { get; }

        public IFileHelperService FileHelper { get; }
    }
}
