using CommonServiceLocator;
using System;
using System.Collections.Generic;
using System.Linq;

namespace savaged.mvvm.Navigation
{
    public class ViewModelLocator
        : ServiceLocatorImplBase, IViewModelLocator
    {
        private readonly IDictionary<Type, IFocusable> _dict;

        public ViewModelLocator(
            IEnumerable<IFocusable> viewModelInstances)
        {
            _dict = new Dictionary<Type, IFocusable>();
            foreach (var vm in viewModelInstances)
            {
                _dict.Add(vm.GetType(), vm);
            }
        }

        public event EventHandler<ViewModelRegistryChangedEventArgs> ViewModelRegistryChanged
            = delegate { };

        public void Register(IFocusable viewModel)
        {
            if (viewModel == null) return;

            var type = viewModel.GetType();

            if (_dict.Keys.Contains(type))
            {
                _dict[type] = viewModel;
            }
            else
            {
                _dict.Add(type, viewModel);
            }
            RaiseViewModelRegistryChanged(viewModel, true);
        }

        public void UnRegister<T>() where T : IFocusable
        {
            var type = typeof(T);
            IFocusable viewModel = null;
            if (_dict.Keys.Contains(type))
            {
                viewModel = _dict[type];
                _dict.Remove(type);
            }
            RaiseViewModelRegistryChanged(viewModel, false);
        }

        protected override IEnumerable<object> DoGetAllInstances(
            Type viewModelType)
        {
            IEnumerable<object> matches;
            try
            {
                matches = _dict.Values.Where(
                    i => i.GetType() == viewModelType ||
                    i.GetType().IsSubclassOf(viewModelType) ||
                    viewModelType.IsAssignableFrom(i.GetType()));
            }
            catch (Exception ex)
            {
                var feedback = FormatActivateAllExceptionMessage(
                    ex, viewModelType);
                throw new ActivationException(feedback);
            }
            return matches;
        }

        protected override object DoGetInstance(
            Type viewModelType, string key)
        {
            object match;
            try
            {
                Type typeSought;
                if (string.IsNullOrEmpty(key))
                {
                    typeSought = viewModelType;
                    match = _dict.Values.Where(
                        t => t.GetType() == typeSought ||
                        typeSought.IsInstanceOfType(t))
                        .FirstOrDefault();
                }
                else
                {
                    if (!key.Contains('.'))
                    {
                        key = $"{viewModelType.Namespace}.{key}";
                    }
                    typeSought = viewModelType.Assembly.GetType(key);
                    match = _dict[typeSought];
                }
            }
            catch (Exception ex)
            {
                var feedback = FormatActivationExceptionMessage(
                    ex, viewModelType, key);
                throw new ActivationException(feedback);
            }
            return match;
        }

        private void RaiseViewModelRegistryChanged(
            IFocusable viewModel, bool registered)
        {
            if (viewModel == null) return;

            ViewModelRegistryChanged?.Invoke(
                this, new ViewModelRegistryChangedEventArgs(viewModel, registered));
        }
    }
}
