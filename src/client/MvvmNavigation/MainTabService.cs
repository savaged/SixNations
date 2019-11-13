using System;
using System.Collections.Generic;
using System.Linq;

namespace savaged.mvvm.Navigation
{
    public class MainTabService 
        : ViewServiceBase<IFocusable>, IMainTabService
    {
        public MainTabService(
            IViewModelLocator viewModelLocatorInstance)
            : this(viewModelLocatorInstance, null, null) { }

        public MainTabService(
            IViewModelLocator viewModelLocatorInstance,
            IDictionary<string, int> mainTabs,
            string homeTab)
            : base(viewModelLocatorInstance)
        {
            if (mainTabs != null)
            {
                MainTabs = mainTabs;
            }
            else
            {
                MainTabs = new Dictionary<string, int>();
            }
            Home = MainTabs.Where(i => i.Key == homeTab)
                .FirstOrDefault();
            Selected = MainTabs.FirstOrDefault();
            Previous = MainTabs.LastOrDefault();
        }

        public int SelectedIndex
        {
            get => Selected.Value;
            set
            {
                if (Selected.Value != value && 
                    MainTabs.Values.Contains(value))
                {
                    Previous = Selected;
                    Selected = MainTabs.Where(i => i.Value == value)
                        .FirstOrDefault();
                    RaiseSelectedIndexChanged(Previous.Value, Selected.Value);
                }
            }
        }

        public event EventHandler<SelectedIndexChangedEventArgs> SelectedIndexChanged = 
            delegate { };

        public KeyValuePair<string, int> Selected { get; private set; }

        public KeyValuePair<string, int> Previous { get; private set; }

        public KeyValuePair<string, int> Home { get; }

        public IDictionary<string, int> MainTabs { get; }

        /// <summary>
        /// IMPORTANT NOTE: If a ViewModel Locator is not configured the
        /// parameter is ignored and only the tab selection is applied.
        /// </summary>
        /// <param name="viewKey"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public override bool? Show(
            string viewKey, object parameter = null)
        {
            var result = base.Show(viewKey, parameter);
            SelectedIndex = MainTabs[viewKey];
            return result;
        }

        public override bool Contains(string viewKey)
        {
            var result = false;
            if (string.IsNullOrEmpty(viewKey))
            {
                result = false;
            }
            else
            {
                var key = viewKey.Replace("View", string.Empty);
                result = MainTabs.Keys.Contains(key);
            }
            return result;
        }

        protected override void Validate(string viewKey)
        {
            if (!Contains(viewKey))
            {
                throw new ArgumentOutOfRangeException(nameof(viewKey),
                    "Navigation can only be to views that are specified " +
                    "in the tabs dictionary supplied at construction!");
            }
        }

        protected override bool? Show(
            Type viewType, IFocusable dataContext = null)
        {
            if (dataContext is null)
            {
                dataContext = GetInitialisedViewModel(viewType.Name);
            }
            return dataContext != null;
        }

        protected override IFocusable GetViewModel(
            string viewKey)
        {
            var viewModels = ViewModelLocatorInstance
                .GetAllInstances<IFocusable>();
            IFocusable value = null;
            if (viewModels.Count() > 0)
            {
                value = viewModels
                    .Where(v => v.GetType().Name == $"{viewKey}ViewModel")
                    .FirstOrDefault();
            }
            if (value == null)
            {
                throw new InvalidOperationException(
                    $"The view: '{viewKey}' was not found " +
                    $"in the {GetType().Name}! Perhaps it " +
                    "has not been registered.");
            }
            return value;
        }

        private void RaiseSelectedIndexChanged(int old, int @new)
        {
            SelectedIndexChanged?.Invoke(
                this, new SelectedIndexChangedEventArgs(old, @new));
            RaisePropertyChanged(nameof(SelectedIndex));
        }
    }
}
