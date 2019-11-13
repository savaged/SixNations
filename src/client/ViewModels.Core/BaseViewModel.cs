using System;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using log4net;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using System.Media;
using savaged.mvvm.ViewModels.Core.Utils;
using GalaSoft.MvvmLight.Messaging;
using System.ComponentModel;
using savaged.mvvm.Navigation;
using savaged.mvvm.Data;
using savaged.mvvm.Core.Interfaces;

namespace savaged.mvvm.ViewModels.Core
{
    public abstract class BaseViewModel : 
        ViewModelBase, IObservableObject, IViewManager
    {
        private readonly ExceptionHandlingService _exceptionHandlingService;
        private IViewModelCommonParams _commonParams;
        private bool? _dialogResult = false;
        private string _title = string.Empty;
        private bool _hasFocus;
        private bool _isDirty;
        private IFocusable _owner;
        
        protected static readonly ILog Log = 
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public BaseViewModel(
            IViewModelCommonParams commonParams,
            IOwnedFocusable owner = null)
        {
            _exceptionHandlingService = new ExceptionHandlingService();

            CommonParams = commonParams;
            Owner = owner;
            HasFocus = Owner?.HasFocus == true;

            ExitCmd = new RelayCommand(OnExit);
            HelpCmd = new RelayCommand<string>(OnHelp);

            PropertyChanged += OnPropertyChanged;
        }

        public override void Cleanup()
        {
            if (Owner != null)
            {
                Owner.PropertyChanged -= OnOwnerPropertyChanged;
            }
            PropertyChanged -= OnPropertyChanged;
            base.Cleanup();
        }

        public IMessenger GetMessengerInstance() => MessengerInstance;

        public IFocusable Owner
        {
            get => _owner;
            set
            {
                _owner = value;
                if (_owner != null)
                {
                    _owner.PropertyChanged += OnOwnerPropertyChanged;
                }
                RaisePropertyChanged();
            }
        }

        public virtual bool IsDirty
        {
            get => _isDirty;
            set => Set(ref _isDirty, value);
        }

        public virtual bool HasFocus
        {
            get => _hasFocus;
            set => Set(ref _hasFocus, value);
        }

        public bool? DialogResult
        {
            get => _dialogResult;
            set
            {
                _dialogResult = value;
                // Always raise change!
                RaisePropertyChanged();
            }
        }

        public virtual string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        public virtual string Identifier
        {
            get => string.Empty;
        }
        

        public INavigationService NavigationService { get; private set; }

        public IModelService ModelService { get; private set; }

        public IViewModelCommonParams CommonParams
        {
            get => _commonParams;
            protected set
            {
                if (value != null)
                {
                    _commonParams = value;
                    ViewState = _commonParams.BusyMonitor;
                    NavigationService = _commonParams.NavigationService;
                    ModelService = _commonParams.ModelService;
                }
            }
        }

        protected bool LeavingConfirmed { get; set; }


        protected new void Set<T>(ref T member, T val, [CallerMemberName] string propertyName = null)
        {
            if (Equals(member, val)) return;
            base.Set(propertyName, ref member, val);
            // Not sure why this next line is needed by I found certain states 
            // and props would not update (e.g. FormVisibility on Cancel)
            if (propertyName.Contains("Visibility")) RaisePropertyChanged(propertyName);
        }

        protected void NotImplementedFeedback(object sender)
        {
            if (sender == null)
            {
                throw new NotSupportedException();
            }
            MessageBox.Show(
                "This " + sender.GetType().Name + " feature is a work in progress. " +
                "If it is essential to your role please see Development and ask them nicely.",
                "Not Implemented", MessageBoxButton.OK, MessageBoxImage.Stop);
        }

        protected virtual void ReactToException(
            object origin, DesktopException ex)
        {
            _exceptionHandlingService.ReactToException(origin, ex);
        }

        protected void DontReactToException(DesktopException ex)
        {
            _exceptionHandlingService.DontReactToException(this, ex);
        }

        protected virtual void ReactToFailure(string msg)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                SystemSounds.Question.Play();
                MessageBox.Show(
                        msg,
                        "Action Failed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
            });
        }

        protected virtual void ReactToSuccess(string msg)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                SystemSounds.Question.Play();
                MessageBox.Show(
                        msg,
                        "Action Successful",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
            });
        }

        public IViewStateViewModel ViewState { get; private set; }


        /// <summary>
        /// Executed from the View in the codebehind
        /// </summary>
        /// <returns></returns>
        public abstract Task<bool> LoadAsync();

        protected virtual Task<bool> LoadLookups()
        {
            return AlwaysTrue();
        }

        public async Task<bool> AlwaysTrue()
        {
            await Task.Yield();
            return true;
        }
        public async Task<bool> AlwaysFalse()
        {
            await Task.Yield();
            return false;
        }

        public ICommand ExitCmd { get; }

        public ICommand HelpCmd { get; }

        public virtual bool CanExecute => ViewState.IsNotBusy;

        protected virtual void OnHelp(string page)
        {
            var vm = new HelpDialogViewModel(page);
            NavigationService.DialogService.Show(vm);
        }

        public virtual void CloseView(bool result = true)
        {
            try
            {
                HasFocus = false;
                Cleanup();
                // This line alone should close the view if it is modal
                DialogResult = result;
                // If not modal fire the event which should have been subscribed to (probably in the view's code behind)
                RaiseRequestClose();
            }
            catch (Exception ex)
            {
                Log.Error($"Unexpected exception during CloseView! {ex}");
            }
        }

        public event Action RequestClose = delegate { };

        public virtual void RaiseRequestClose()
        {
            RequestClose?.Invoke();
        }

        public void OnExit()
        {
            // If the view has not been loaded modal
            CloseView();
        }

        public bool ConfirmLeaving()
        {
            if (!LeavingConfirmed && IsDirty)
            {
                var confirmation = MessageBox.Show(
                    "Unsaved changes detected! Are you sure?",
                    "Confirm Leaving Current Context",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                    );
                if (confirmation != MessageBoxResult.Yes)
                {
                    LeavingConfirmed = false;
                    return LeavingConfirmed;
                }
            }
            LeavingConfirmed = true;
            return LeavingConfirmed;
        }

        public virtual bool ConfirmDelete(
            string additionalMsg = null, bool isArchiveOnly = false)
        {
            var msg = "Are you sure? ";
            if (!string.IsNullOrEmpty(additionalMsg))
            {
                msg += additionalMsg;
            }
            var title = "Confirm ";
            if (isArchiveOnly)
            {
                title += "Archive";
            }
            else
            {
                title += "Delete";
            }
            var confirmation = MessageBox.Show(
                msg,
                title,
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            if (confirmation != MessageBoxResult.Yes)
            {
                return false;
            }
            return true;
        }

        public event Action RequestReload = delegate { };

        protected void RaiseRequestReload()
        {
            RequestReload?.Invoke();
        }

        protected ILookupServiceLocator GetLookupServiceLocator()
        {
            var value = CommonParams?.LookupServiceLocator ??
                    throw new ArgumentNullException(
                        nameof(IViewModelCommonParams.LookupServiceLocator));
            return value;
        }


        private void OnOwnerPropertyChanged(
            object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(HasFocus))
            {
                HasFocus = Owner.HasFocus;
            }
        }

        private void OnPropertyChanged(
            object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewState.IsBusy))
            {
                RaiseCanExecuteChangedForAllRelayCommands();
            }
            if (e.PropertyName == nameof(Owner))
            {
                HasFocus = Owner?.HasFocus == true;
            }
        }
        protected void RaiseCanExecuteChangedForAllRelayCommands()
        {
            var type = GetType();
            var properties = type.GetProperties();
            foreach (var p in properties)
            {
                if (p.PropertyType.Name.StartsWith(nameof(RelayCommand)))
                {
                    var cmd = p.GetValue(this);
                    if (cmd != null)
                    {
                        var t = cmd.GetType();
                        var m = t.GetMethod("RaiseCanExecuteChanged");
                        m.Invoke(cmd, null);
#if DEBUG
                        Console.WriteLine("RaiseCanExecuteChanged called for: " + p.Name);
#endif
                    }
                }
            }
        }

    }
}
