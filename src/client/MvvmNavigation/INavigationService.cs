namespace savaged.mvvm.Navigation
{
    public interface INavigationService 
        : GalaSoft.MvvmLight.Views.INavigationService
    {
        IMainTabService MainTabService { get; }

        IDialogService DialogService { get; }

        void GoHome();
    }
}