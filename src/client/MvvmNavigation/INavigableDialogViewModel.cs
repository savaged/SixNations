using System.ComponentModel;

namespace savaged.mvvm.Navigation
{
    public interface INavigableDialogViewModel 
        : IFocusable, INotifyPropertyChanged
    {
        bool? DialogResult { get; set; }

        void RaiseRequestClose();
    }
}