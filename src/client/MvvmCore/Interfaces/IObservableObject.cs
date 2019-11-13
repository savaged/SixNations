using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace savaged.mvvm.Core.Interfaces
{
    public interface IObservableObject : INotifyPropertyChanged
    {
        void RaisePropertyChanged(
            [CallerMemberName] string propertyName = "");
    }
}
