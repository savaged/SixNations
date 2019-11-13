namespace savaged.mvvm.Navigation
{
    public interface IViewService
    {
        bool? Show(string viewKey, object args = null);

        bool? Show(IFocusable viewModel);

        bool Contains(string viewKey);
    }
}