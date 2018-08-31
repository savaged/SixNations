using System;
using System.Collections.Generic;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using SixNations.Desktop.Constants;
using SixNations.Desktop.Interfaces;

namespace SixNations.Desktop.Services
{
    public class NavigationService : ObservableObject, INavigationService
    {
        private readonly IDictionary<string, int> _navItems;
        private HamburgerNavItemsIndex _previousNavItem;
        private HamburgerNavItemsIndex _currentNavItem;

        public NavigationService()
        {
            _navItems = new Dictionary<string, int>
            {
                {
                    HamburgerNavItemsIndex.Login.ToString(),
                    (int)HamburgerNavItemsIndex.Login
                },
                {
                    HamburgerNavItemsIndex.Requirements.ToString(),
                    (int)HamburgerNavItemsIndex.Requirements
                }
            };
        }

        public string CurrentPageKey => _currentNavItem.ToString();

        public void GoBack()
        {
            NavigateTo(_previousNavItem.ToString());
        }

        public void NavigateTo(string pageKey)
        {
            NavigateTo(pageKey, null);
        }

        public void NavigateTo(string pageKey, object parameter)
        {
            if (_navItems.ContainsKey(pageKey))
            {
                if (parameter != null)
                {
                    var mainVM = App.Current.MainWindow.DataContext;
                    var pageVmType = GetType().Assembly.GetType(
                        $"{mainVM.GetType().Namespace}.{pageKey}ViewModel");
                    var pageVm = ServiceLocator.Current.GetInstance(pageVmType);
                    if (pageVm is IParameterised)
                    {
                        ((IParameterised)pageVm).Initialise(parameter);
                    }
                }
                SelectedIndex = _navItems[pageKey];
            }
        }        

        public int SelectedIndex
        {
            get => (int)_currentNavItem;
            set
            {
                _previousNavItem = _currentNavItem;
                _currentNavItem = (HamburgerNavItemsIndex)value;
                RaisePropertyChanged();
                RaiseSelectedIndexChanged();
            }
        }

        private void RaiseSelectedIndexChanged()
        {
            var handler = SelectedIndexChanged;
            handler?.Invoke(this, new SelectedIndexChangedEventArgs(
                (int)_previousNavItem, (int)_currentNavItem));
        }

        public event EventHandler<EventArgs> SelectedIndexChanged;
    }

    public class SelectedIndexChangedEventArgs 
        : EventArgs
    {
        public SelectedIndexChangedEventArgs(int oldValue, int newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        public int OldValue { get; }

        public int NewValue { get; }
    }
}
