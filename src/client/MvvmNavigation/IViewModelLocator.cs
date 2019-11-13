using CommonServiceLocator;
using System;

namespace savaged.mvvm.Navigation
{
    public interface IViewModelLocator : IServiceLocator
    {
        event EventHandler<ViewModelRegistryChangedEventArgs> ViewModelRegistryChanged;
        void Register(IFocusable viewModel);
        void UnRegister<T>() where T : IFocusable;
    }
}