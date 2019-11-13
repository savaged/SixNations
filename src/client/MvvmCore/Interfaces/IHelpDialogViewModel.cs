using System;

namespace savaged.mvvm.Core.Interfaces
{
    public interface IHelpDialogViewModel 
    {
        string Source { get; }

        event Action RequestClose;
    }
}