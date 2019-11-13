using savaged.mvvm.Navigation;
using System.Windows.Input;

namespace savaged.mvvm.Core.Interfaces
{
    public interface IModelObjectDialogViewModel<T> 
        : ISelectedItemViewModel<T>, 
        IDialogResultViewModel,
        INavigableDialogViewModel,
        IReloadable
        where T : IObservableModel, new()
    {
        IModelObjectDialogViewModel<T> Template();

        ILogDialogViewModel<T> LogDialogViewModel { get; }

        string Title { get; set; }

        bool CanDelete { get; }

        bool CanSave { get; }

        ICommand FormCancelCmd { get; }
        ICommand FormDeleteCmd { get; }
        ICommand FormSaveCmd { get; }

        bool IsReadOnly { get; set; }

        bool ConfirmDelete(
            string additionalMsg = null, bool isArchiveOnly = false);
    }
}