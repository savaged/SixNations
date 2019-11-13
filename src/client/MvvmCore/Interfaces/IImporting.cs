using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Input;

namespace savaged.mvvm.Core.Interfaces
{
    public interface IImporting<T, C>
        where T : IObservableModel
        where C : IChildCollection<T>
    {
        IViewModelCommonParams CommonParams { get; }

        void CloseView(bool result = true);

        bool CanExecuteClear { get; }
        bool CanSave { get; }
        ICommand FormSaveCmd { get; }
        ICommand ClearCmd { get; }
        ICommand PasteCmd { get; }

        IObservableModel Parent { get; }

        ObservableCollection<T> Index { get; }

        C Import { get; }

        Task UpdateIndexFromDataTable(DataTable dt);

        Task<bool> FormSave();
    }
}