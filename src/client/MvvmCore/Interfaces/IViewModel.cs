using savaged.mvvm.Navigation;
using System.Threading.Tasks;
using System.Windows.Input;

namespace savaged.mvvm.Core.Interfaces
{
    public interface IViewModel : IOwnedFocusable
    {
        IViewStateViewModel ViewState { get; }

        IViewModelCommonParams CommonParams { get; }

        bool IsDirty { get; set; }

        /// <summary>
        /// This is called from the view's code-behind
        /// just after it finishes rendering.
        /// </summary>
        /// <returns></returns>
        Task<bool> LoadAsync();


        // TODO move some of these up to IViewManager

        INavigationService NavigationService { get; }

        void Cleanup();

        bool ConfirmLeaving();

        void CloseView(bool result = true);

        ICommand HelpCmd { get; }

        string Identifier { get; }
    }
}
