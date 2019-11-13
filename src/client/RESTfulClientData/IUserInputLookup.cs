using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace savaged.mvvm.Data
{
    public interface IUserInputLookup
    {
        string NewItem { get; set; }
        string SelectedItem { get; set; }

        event PropertyChangedEventHandler PropertyChanged;

        void Add(string value);
        void RaisePropertyChanged([CallerMemberName] string propertyName = "");
    }
}