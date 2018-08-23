using System;
using System.ComponentModel;

namespace SixNations.Desktop.Interfaces
{
    public interface ISelectedIndexManager : INotifyPropertyChanged
    {
        int SelectedIndex { get; set; }
    }
}
