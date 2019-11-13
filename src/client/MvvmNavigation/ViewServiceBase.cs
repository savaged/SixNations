using CommonServiceLocator;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace savaged.mvvm.Navigation
{
    public abstract class ViewServiceBase<T> : ObservableObject
        where T : IFocusable
    {
        private readonly IDictionary<string, Type> _viewTypes;

        public ViewServiceBase(
            IViewModelLocator viewModelLocatorInstance)
        {
            ViewModelLocatorInstance = viewModelLocatorInstance ??
                ServiceLocator.Current;

            _viewTypes = new Dictionary<string, Type>();
            if (!ViewModelBase.IsInDesignModeStatic)
            {                
                var viewsAssembly = GetViewsAssembly();
                var runtimeTypes = viewsAssembly.GetTypes();
                foreach (var rt in runtimeTypes)
                {
                    if (rt?.BaseType == typeof(Window)
                        || rt?.BaseType?.BaseType == typeof(Window)
                        || rt?.BaseType == typeof(UserControl)
                        || rt?.BaseType?.BaseType == typeof(UserControl))
                    {
                        var key = rt.Name;
                        if (key.EndsWith("View"))
                        {
                            key = key.Remove(key.Length - "View".Length);
                        }
                        _viewTypes.Add(key, rt);
                    }
                }
                if (_viewTypes.Count() == 0)
                {
                    throw new InvalidOperationException("No views found!");
                }
            }
        }

        public abstract bool Contains(string viewKey);

        /// <summary>
        /// This should be overridden for tab navigation.
        /// Add "modal" to the view's Tag to make the window modal.
        /// </summary>
        /// <param name="viewKey"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public virtual bool? Show(string viewKey, object parameter = null)
        {
            Validate(viewKey);
            Type viewType;
            if (_viewTypes.Keys.Contains(viewKey))
            {
                viewType = _viewTypes[viewKey];
            }
            else if (_viewTypes.Keys.Contains($"{viewKey}"))
            {
                viewType = _viewTypes[$"{viewKey}"];
            }
            else
            {
                throw new ArgumentException(
                    $"The value [{viewKey}] does not appear to have a " +
                    $"matching Window or UserControl!", nameof(viewKey));
            }
            var dataContext = GetInitialisedViewModel(viewKey, parameter);

            var result = Show(viewType, dataContext);
            return result;
        }

        /// <summary>
        /// NOTE: Add "modal" to the view's Tag to make the window modal.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public virtual bool? Show(IFocusable viewModel)
        {
            if (viewModel is null)
            {
                return null;
            }
            var viewType = GetViewType(viewModel);
            var result = Show(viewType, viewModel);
            return result;
        }

        protected IServiceLocator ViewModelLocatorInstance { get; }

        protected abstract bool? Show(
            Type viewType, IFocusable dataContext = null);

        protected abstract void Validate(string viewKey);

        protected abstract T GetViewModel(string viewKey);

        protected Type GetViewType(IFocusable viewModel)
        {
            var vmName = viewModel.GetType().Name;
            var viewKey = vmName.Substring(0, vmName.IndexOf("ViewModel"));
            var value = _viewTypes[viewKey];
            return value;
        }

        protected IFocusable GetInitialisedViewModel(
            string viewKey, object parameter = null)
        {
            var vm = GetViewModel(viewKey);
            if (vm == null)
            {
                throw new InvalidOperationException(
                    "The view model should be set by here!");
            }
            vm.HasFocus = true;
            if (parameter != null && vm is IInitialised iVm)
            {
                iVm.Initialise(parameter);
            }
            return vm;
        }

        protected Type GetViewType(string viewName)
        {
            if (!_viewTypes.Keys.Contains(viewName))
            {
                return null;
            }
            var value = _viewTypes[viewName];
            return value;
        }

        private Assembly GetViewsAssembly()
        {
            var value = GetReferencedAssembly("Views");
            if (value is null)
            {
                value = Assembly.GetEntryAssembly();
            }
            return value;
        }

        private Assembly GetReferencedAssembly(string assemblyKey)
        {
            var curr = GetType().Assembly.GetName().Name;
            var i = curr.LastIndexOf('.');
            var prefix = curr.Substring(0, i);
            var query = $"{prefix}.{assemblyKey}";
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var value = assemblies.Where(
                a => a.FullName.StartsWith(query))
                .FirstOrDefault();
            return value;
        }

    }
}
