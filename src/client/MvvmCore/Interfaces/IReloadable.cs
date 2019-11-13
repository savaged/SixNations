using savaged.mvvm.Navigation;
using System.Threading.Tasks;
using System.Windows.Input;

namespace savaged.mvvm.Core.Interfaces
{
    public interface IReloadable : IOwnedFocusable
    {
        IViewStateViewModel ViewState { get; }

        Task Reload();

        ICommand ReloadCmd { get; }
    }
}
