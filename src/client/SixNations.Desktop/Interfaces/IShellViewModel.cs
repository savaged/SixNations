using System;

namespace SixNations.Desktop.Interfaces
{
    public interface IShellViewModel
    {
        ISelectedIndexManager SelectedIndexManager { get; }

        bool IsFullScreen { get; }

        event EventHandler<IIsFullScreenChangedEventArgs> IsFullScreenChanged;
    }
}
