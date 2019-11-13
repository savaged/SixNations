using System.ComponentModel;

namespace savaged.mvvm.Navigation
{
    public interface ISelectedIndexService : INotifyPropertyChanged
    {
        int SelectedIndex { get; set; }
    }
}
