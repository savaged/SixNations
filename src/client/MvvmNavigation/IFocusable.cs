using System.ComponentModel;

namespace savaged.mvvm.Navigation
{
    public interface IFocusable : INotifyPropertyChanged
    {
        bool HasFocus { get; set; }
    }
}