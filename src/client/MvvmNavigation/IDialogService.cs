namespace savaged.mvvm.Navigation
{
    public interface IDialogService : IViewService
    {
        void CloseAll();

        bool IsModal(IFocusable viewModel);
    }
}
