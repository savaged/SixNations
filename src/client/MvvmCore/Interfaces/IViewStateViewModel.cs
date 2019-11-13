using System;

namespace savaged.mvvm.Core.Interfaces
{
    public interface IViewStateViewModel : IBusyStateRegistry
    {
        bool IsNotBusy { get; }

        IObservableModel Clipboard { get; set; }
        bool CanPaste(Type T);
    }
}