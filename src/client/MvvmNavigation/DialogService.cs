using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace savaged.mvvm.Navigation
{
    /// <summary>
    /// Add "modal" to the view's Tag to show a dialog modal
    /// </summary>
    public class DialogService 
        : ViewServiceBase<IFocusable>, IDialogService
    {
        private readonly IDictionary<string, IFocusable> _VMs;

        public DialogService(
            IViewModelLocator viewModelLocatorInstance) 
            : base(viewModelLocatorInstance)
        {
            _VMs = new Dictionary<string, IFocusable>();

            if (!ViewModelBase.IsInDesignModeStatic)
            {
                var viewModels = ViewModelLocatorInstance
                    .GetAllInstances<IFocusable>();
                foreach (var vm in viewModels)
                {
                    _VMs.Add(vm.GetType().Name, vm);
                }
                var dialogViewModels = ViewModelLocatorInstance
                    .GetAllInstances<INavigableDialogViewModel>();
                foreach (var dvm in dialogViewModels)
                {
                    _VMs.Add(dvm.GetType().Name, dvm);
                }

                viewModelLocatorInstance.ViewModelRegistryChanged +=
                    OnViewModelRegistryChanged;
            }
        }

        public override bool Contains(string viewKey)
        {
            return _VMs.Keys.Contains($"{viewKey}ViewModel");
        }

        public void CloseAll()
        {
            var windows = GetWindows();
            foreach (var window in windows)
            {
                if (window.Owner == null)
                {
                    continue;
                }
                if (window.IsLoaded)
                {
                    if (IsModal(window))
                    {
                        window.DialogResult = false;
                    }
                    window.Close();
                }
            }
        }

        public bool IsModal(IFocusable viewModel)
        {
            var windowType = GetViewType(viewModel);
            var value = IsModal(windowType);
            return value;
        }

        protected override bool? Show(
            Type viewType, IFocusable dataContext = null)
        {
            Window window = null;
            if (!IsInitialized(viewType))
            {
                window = GetWindow(viewType);
            }
            if (window == null)
            {
                window = GetWindowInstance(viewType);
            }
            window.Owner = Application.Current.MainWindow;
            window.DataContext = null;
            if (dataContext is null)
            {
                dataContext = GetInitialisedViewModel(viewType.Name);
            }
            if (dataContext is INavigableDialogViewModel vm)
            {
                vm.DialogResult = null;
                vm.PropertyChanged += OnViewModelPropertyChanged;
            }
            window.DataContext = dataContext;
            window.Activate();

            bool? result = true;
            if (IsModal(window))
            {
                result = window.ShowDialog();
            }
            else
            {
                window.Show();
            }
            return result;
        }

        protected override void Validate(string viewKey)
        {
            if (!Contains(viewKey))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(viewKey),
                    "Matching view model not found!");
            }
        }

        protected override IFocusable GetViewModel(string viewKey)
        {
            var value = _VMs[$"{viewKey}ViewModel"];
            if (value == null)
            {
                throw new InvalidOperationException(
                    $"The view: '{viewKey}' was not found " +
                    $"in the {GetType().Name}!");
            }
            return value;
        }

        private bool IsInitialized(Type windowType)
        {
            var value = false;
            var window = GetWindow(windowType);
            value = window?.IsInitialized == true;
            return value;
        }

        private bool IsModal(Type windowType)
        {
            var window = GetWindow(windowType);
            var value = window != null &&
                window.Tag != null &&
                window.Tag.ToString().ToLower().Contains("modal");
            return value;
        }

        private bool IsModal(Window window)
        {
            var value = window != null &&
                window.Tag != null &&
                window.Tag.ToString().ToLower().Contains("modal");
            return value;
        }

        private Window GetWindow(Type windowType)
        {
            if (windowType == null) return null;
            var value = GetWindows().FirstOrDefault(
                w => w.GetType() == windowType);
            return value;
        }

        private Window GetWindowInstance(Type windowType)
        {
            var value = (Window)Activator.CreateInstance(windowType);
            return value;
        }

        private IEnumerable<Window> GetWindows()
        {
            var value = Application.Current.Windows.Cast<Window>();
            return value;
        }

        private void Close(string windowName, bool? dialogResult)
        {
            var windowType = GetViewType(windowName);
            Close(windowType, dialogResult);
        }

        private void Close(Type windowType, bool? dialogResult)
        {
            var window = GetWindow(windowType);
            if (window != null)
            {
                var isModal = IsModal(window);
                if (isModal)
                {
                    window.DialogResult = dialogResult;
                }
                else
                {
                    window.Close();
                }
            }
        }

        private void Close(Window window, bool? dialogResult)
        {
            if (window != null)
            {
                var isModal = IsModal(window);
                if (isModal)
                {
                    window.DialogResult = dialogResult;
                }
                else
                {
                    window.Close();
                }
            }
        }

        private void OnViewModelPropertyChanged(
            object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName ==
                nameof(INavigableDialogViewModel.DialogResult))
            {
                if (sender is INavigableDialogViewModel vm &&
                    vm.DialogResult != null)
                {
                    var vmName = vm.GetType().Name;
                    var windowName = vmName.Substring(
                        0, vmName.IndexOf("ViewModel"));
                    Close(windowName, vm.DialogResult);
                    vm.PropertyChanged -= OnViewModelPropertyChanged;
                }
            }
        }

        private void OnViewModelRegistryChanged(
            object sender, ViewModelRegistryChangedEventArgs e)
        {
            var vm = e.ViewModel;
            if (vm == null || !(vm is INavigableDialogViewModel)) return;

            var key = vm.GetType().Name;

            if (e.Registered)
            {                
                _VMs.Add(key, vm);
            }
            else
            {
                _VMs.Remove(key);
            }
        }

    }
}
