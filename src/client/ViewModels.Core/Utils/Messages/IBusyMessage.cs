using System;

namespace savaged.mvvm.ViewModels.Core
{
    public interface IBusyMessage
    {
        string CallerMember { get; }
        Type CallerType { get; }
        bool IsBusy { get; }
    }

}
