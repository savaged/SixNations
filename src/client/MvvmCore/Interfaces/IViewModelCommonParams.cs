using savaged.mvvm.Data;
using savaged.mvvm.Navigation;

namespace savaged.mvvm.Core.Interfaces
{
    public interface IViewModelCommonParams
    {
        IViewStateViewModel BusyMonitor { get; }
        INavigationService NavigationService { get; }
        IModelService ModelService { get; }
        ILookupServiceLocator LookupServiceLocator { get; }
        IFileService FileService { get; }
        IFileHelperService FileHelper { get; }
    }
}