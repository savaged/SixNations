using savaged.mvvm.Core;
using savaged.mvvm.Core.Interfaces;
using savaged.mvvm.Data;
using savaged.mvvm.Navigation;
using savaged.mvvm.ViewModels.Core.Utils;
using savaged.mvvm.ViewModels.Core.Utils.Messages;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace savaged.mvvm.ViewModels.Core
{
    public abstract class ModelObjectDialogViewModel<T>
        : SelectedItemViewModel<T>, IModelObjectDialogViewModel<T>
        where T : IObservableModel, new()
    {
        private readonly IReloadable _reloadComponent;
        private bool _isReadOnly;

        public ModelObjectDialogViewModel(
            IViewModelCommonParams commonParams,
            IOwnedFocusable owner)
            : base(commonParams, owner)
        {
            CtorArgs = new object[]
            {
                commonParams, owner
            };

            if (Owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }        

            _reloadComponent = new ReloadComponent(this, MessengerInstance);

            FormCancelCmd = new RelayCommand(
                OnFormCancel, () => CanExecute);
            FormSaveCmd = new RelayCommand(OnFormSave, () => CanSave);
            FormDeleteCmd = new RelayCommand(
                OnFormDelete, () => CanDelete);

            ViewLogCmd = new RelayCommand(OnViewLog, () => CanViewLog);
        }

        public object[] CtorArgs { get; protected set; }

        public IModelObjectDialogViewModel<T> Template()
        {
            object obj = null;

            if (CtorArgs != null)
            {
                obj = Activator.CreateInstance(GetType(), CtorArgs);
            }
            else
            {
                obj = Activator.CreateInstance(GetType());
            }
            var value = obj as IModelObjectDialogViewModel<T>;

            return value;
        }

        public override void Cleanup()
        {
            if (ModelObject != null)
            {
                ModelObject.PropertyChanged -= OnModelObjectPropertyChanged;
            }
            base.Cleanup();
        }

        public override void Seed(T modelObject, IObservableModel parent = null)
        {
            IsDirty = false;

            ModelObject = modelObject.Clone();
            if (ModelObject?.Equals(modelObject) == false)
            {
                throw new InvalidOperationException(
                    $"The clone of {nameof(modelObject)} is degraded!");
            }
            Parent = parent.Clone();
            if (Parent?.Equals(parent) == false)
            {
                throw new InvalidOperationException(
                    $"The clone of {nameof(parent)} is degraded!");
            }
        }

        public async override Task<bool> LoadAsync()
        {
            if (!HasFocus) return false;

            MessengerInstance.Send(new BusyMessage(true, this));
            try
            {
                ModelObjectAtLoad = ModelObject.Clone();

                LeavingConfirmed = false;

                if (ModelObject.IsNullOrNew())
                {
                    await CreateAsync();
                }
                else
                {
                    var result = await EditAsync();
                    if (!result)
                    {
                        CloseView(false);
                    }
                }
                RaisePropertyChanged(nameof(Title));
                await LoadLookups();

                if (this is ILoggedViewModel<T> loggedVm)
                {
                    loggedVm.InitialiseLogDialogViewModel(ModelObject);
                }
            }
            catch (ApiAuthException)
            {
                ModelObject = default;
            }
            catch (ApiValidationException ex)
            {
                ReactToException(this, ex);
            }
            finally
            {
                MessengerInstance.Send(new BusyMessage(false, this));
            }
            if (ModelObject == null)
            {
                CloseView(false);
                return false;
            }

            ModelObject.PropertyChanged += OnModelObjectPropertyChanged;

            // Not sure why this is needed but it is!?!
            RaisePropertyChanged(nameof(CanExecute));
            RaisePropertyChanged(nameof(CanEdit));
            RaisePropertyChanged(nameof(CanDelete));
            RaisePropertyChanged(nameof(CanSave));
            RaisePropertyChanged(nameof(CanViewLog));

            return true;
        }

        public async Task Reload()
        {
            await LoadAsync();
        }

        public override string Title
        {
            get => string.IsNullOrEmpty(base.Title) ?
                ModelObject?.Identifier : base.Title;
            set => base.Title = value;
        }

        public virtual bool IsReadOnly
        {
            get => _isReadOnly;
            set => Set(ref _isReadOnly, value);
        }

        public event EventHandler<IDialogClosedEventArgs> DialogClosed =
            delegate { };

        public override bool CanEdit => !IsReadOnly && base.CanEdit;

        public virtual bool CanSave => !IsReadOnly && 
            ModelObject != null && IsDirty;

        public virtual bool CanViewLog => CanExecute &&
            !ModelObject.IsNullOrNew();

        public ICommand ReloadCmd => _reloadComponent.ReloadCmd;

        public ICommand FormCancelCmd { get; private set; }

        public ICommand FormSaveCmd { get; private set; }

        public ICommand FormDeleteCmd { get; private set; }

        public ICommand ViewLogCmd { get; set; }

        public ILogDialogViewModel<T> LogDialogViewModel { get; protected set; }

        protected IPollingService KeepAlivePollingService { private get; set; }

        protected IDictionary<string, object> FormActionArgs { get; set; }

        protected virtual bool KeepAlive => false;

        protected T ModelObjectAtLoad { get; set; }

        protected virtual async Task CreateAsync()
        {
            if (ModelObject?.IsUnlocked == true)
            {
                return;
            }

            if (KeepAlive)
            {
                KeepAlivePollingService?.Start();
            }
            var created = await ModelService.CreateAsync<T>(
                AuthUser.Current, Parent);

            if (created != null)
            {
                created.IsUnlocked = true;
            }
            ModelObject = created;
        }

        protected virtual async Task<bool> EditAsync()
        {
            try
            {
                if (ModelObject == null)
                {
                    throw new InvalidOperationException(
                        $"{nameof(ModelObject)} null at Edit!");
                }
                if (ModelObject.IsUnlocked)
                {
                    return true;
                }

                if (KeepAlive)
                {
                    KeepAlivePollingService?.Start();
                }
                var unlocked = await ModelService.EditAsync(
                    AuthUser.Current, ModelObject);

                if (unlocked != null)
                {
                    unlocked.IsUnlocked = true;
                }
                ModelObject = unlocked;
            }
            catch (ApiAuthException)
            {
                return false;
            }
            catch (ApiRecordLockedException ex)
            {
                ReactToException(this, ex);
                return false;
            }
            if (ModelObject.IsNullOrNew())
            {
                throw new InvalidOperationException(
                    "The model object should have a positive Id!");
            }
            var result = ModelObject.IsUnlocked;
            return result;
        }

        protected async Task<bool> QuietFormSave()
        {
            var result = false;
            MessengerInstance.Send(new BusyMessage(true, this));
            try
            {
                if (ModelObject.IsNew)
                {
                    // IMPORTANT NOTE:
                    // The Parent should not be required because the create
                    // should have added the parent's Id property in the model
                    var stored = await ModelService.StoreAsync(
                        AuthUser.Current,
                        ModelObject,
                        args: FormActionArgs);
                    ModelObject = stored;
                }
                else
                {
                    var updated = await ModelService.UpdateAsync(
                        AuthUser.Current, ModelObject);
                    ModelObject = updated;
                }
                KeepAlivePollingService?.Stop();

                IsDirty = false;
                result = true;
            }
            finally
            {
                MessengerInstance.Send(new BusyMessage(false, this));
            }
            return result;
        }

        protected virtual async Task<bool> FormSave()
        {
            if (ModelObject == null)
            {
                throw new InvalidOperationException(
                    "The model object should be properly set by here!");
            }
            var result = false;
            MessengerInstance.Send(new BusyMessage(true, this));
            try
            {
                var isAddition = ModelObject.IsNew;
                var old = ModelObject.Clone();

                result = await QuietFormSave();

                if (result)
                {
                    MessengerInstance.Send(new ModelObjectPersistedMessage<T>(
                        this, 
                        Owner as IOwnedFocusable, 
                        old, 
                        ModelObject, 
                        Parent,
                        isAddtion: isAddition));
                }
            }
            catch (GatewayException ex) 
            when (ex is ApiValidationException || ex is ApiRecordLockedException)
            {
                ReactToException(this, ex);
                result = false;
            }
            catch (ApiAuthException)
            {
                CloseView(false);
                result = false;
            }
            finally
            {
                MessengerInstance.Send(new BusyMessage(false, this));
            }
            if (result)
            {
                await ReactToSuccessfulSave();
            }
            return result;
        }

        protected async virtual Task ReactToSuccessfulSave()
        {
            CloseView(true);
            await Task.Yield();
        }

        protected override async Task<bool> FormDelete()
        {
            var result = false;

            MessengerInstance.Send(new BusyMessage(true, this));
            try
            {
                result = await QuietDeleteAsync();
            }
            catch (DesktopException ex)
            {
                ReactToException(this, ex);
            }
            finally
            {
                MessengerInstance.Send(new BusyMessage(false, this));
            }
            CloseView(result);
            return result;
        }

        protected async Task<bool> QuietDeleteAsync()
        {
            var result = false;
            result = await base.FormDelete();
            KeepAlivePollingService?.Stop();
            return result;
        }

        protected virtual async Task FormCancel()
        {
            if (!ConfirmLeaving()) return;

            MessengerInstance.Send(new BusyMessage(true, this));
            try
            {
                await QuietCancelAsync();
            }
            catch (DesktopException ex)
            {
                ReactToException(this, ex);
            }
            finally
            {
                MessengerInstance.Send(new BusyMessage(false, this));
            }
            CloseView(false);
        }
        protected async Task QuietCancelAsync()
        {
            if (!ModelObject.IsNullOrNew())
            {
                await ModelService
                    .UnlockAsync(AuthUser.Current, ModelObject);
            }
            ModelObject = ModelObjectAtLoad;

            KeepAlivePollingService?.Stop();
        }

        private void RaiseDialogClosed(bool forceDialogResultSuccess)
        {
            var handler = DialogClosed;
            var dialogResult = forceDialogResultSuccess
                ? true
                : DialogResult;

            var args = new DialogClosedEventArgs(dialogResult);
            handler?.Invoke(this, args);
        }

        private async void OnFormSave()
        {
            await FormSave();
        }

        private async void OnFormCancel()
        {
            await FormCancel();
        }

        private async void OnFormDelete()
        {
            await FormDelete();
        }

        private void OnViewLog()
        {
            if (LogDialogViewModel != null)
            {
                NavigationService.DialogService.Show(LogDialogViewModel);
            }
        }

        /// <summary>
        /// Override to return bool for Window event
        /// void OnClosing(object sender, CancelEventArgs e)
        /// {
        ///    if (DataContext is IDialogViewModel dialog)
        ///    {
        ///       e.Cancel = dialog.OnClosing();
        ///    }
        /// }
        /// </summary>
        /// <returns></returns>
        public virtual bool OnClosing(
            bool forceDialogResultSuccess = false)
        {
            var cancel = false;

            HasFocus = false;

            RaiseDialogClosed(forceDialogResultSuccess);

            return cancel;
        }

        private void OnModelObjectPropertyChanged(
            object sender, PropertyChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.PropertyName))
            {
                switch (e.PropertyName)
                {
                    case nameof(ModelObject.Identifier):
                        Title = ModelObject.Identifier;
                        break;
                    default:
                        IsDirty = true;
                        break;
                }
                RaisePropertyChanged(e.PropertyName);
            }
        }

    }
}
