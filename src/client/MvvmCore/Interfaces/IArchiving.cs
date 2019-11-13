using savaged.mvvm.Data;
using savaged.mvvm.Navigation;
using System.Windows.Input;

namespace savaged.mvvm.Core.Interfaces
{
    public interface IArchiving<T> : IOwnedFocusable
        where T : IArchiveable, new()
    {
        ICommand ArchiveCmd { get; }
        bool CanArchive { get; }
        bool CanExecuteRestoreArchived { get; }
        bool IsArchived { get; }
        ICommand RestoreArchivedCmd { get; }

        T ModelObject { get; set; }
        IObservableModel Parent { get; }
        IViewStateViewModel ViewState { get; }
        IModelService ModelService { get; }
        void CloseView(bool result = true);
    }
}