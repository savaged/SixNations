using System;

namespace savaged.mvvm.Core.Interfaces
{
    public interface IDialogResultViewModel
    {
        bool OnClosing(bool forceDialogResultSuccess = false);

        event EventHandler<IDialogClosedEventArgs> DialogClosed;
    }
}
