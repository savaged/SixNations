using savaged.mvvm.Core.Interfaces;
using System;

namespace savaged.mvvm.ViewModels.Core.Utils
{
    public class DialogClosedEventArgs : EventArgs, IDialogClosedEventArgs
    {
        public DialogClosedEventArgs(bool? dialogResult)
        {
            DialogResult = dialogResult;
        }

        public bool? DialogResult { get; }
    }
}
