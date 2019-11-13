using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace savaged.mvvm.Data
{
    public class UserInputLookup 
        : List<string>, IUserInputLookup
    {
        private string _selectedItem;

        public UserInputLookup() { }

        public UserInputLookup(string selectedItem)
        {
            Add(selectedItem);
            _selectedItem = selectedItem;
        }

        public string SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                RaisePropertyChanged();
            }
        }

        public string NewItem
        {
            get => SelectedItem;
            set
            {
                if (value.ToLower().StartsWith("delete:"))
                {
                    value = value.Substring(7, value.Length - 7).TrimStart();
                    Remove(value);
                    SelectedItem = null;
                    RaisePropertyChanged(GetType().Name);
                }
                else
                {
                    Add(value);
                    SelectedItem = value;
                }
                RaisePropertyChanged();
            }
        }

        public new void Add(string value)
        {
            if (!string.IsNullOrEmpty(value) && !string.IsNullOrWhiteSpace(value))
            {
                if (!Contains(value))
                {
                    base.Add(value);
                }
            }
        }
        
        #region Property Changed Event
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public void RaisePropertyChanged(
            [CallerMemberName] string propertyName = "")
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
