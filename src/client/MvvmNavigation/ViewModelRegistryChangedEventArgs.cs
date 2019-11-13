using System;

namespace savaged.mvvm.Navigation
{
    public class ViewModelRegistryChangedEventArgs : EventArgs
    {
        public ViewModelRegistryChangedEventArgs(
            IFocusable viewModel, bool registered)
        {
            ViewModel = viewModel;
            Registered = registered;
        }

        public IFocusable ViewModel { get; }

        public bool Registered { get; }
    }
}
