using System;

namespace savaged.mvvm.Core.Interfaces
{
    public interface IViewManager : IViewModel
    {
        event Action RequestClose;

        event Action RequestReload;
    }
}
