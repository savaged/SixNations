using savaged.mvvm.Core;
using savaged.mvvm.Core.Interfaces;
using savaged.mvvm.Data;
using savaged.mvvm.Navigation;
using savaged.mvvm.ViewModels.Core.Utils;
using savaged.mvvm.ViewModels.Core.Utils.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Windows;
using System.Windows.Input;

namespace savaged.mvvm.ViewModels.Core
{
    public sealed class ArchivingComponent<T>
        : ViewModelBase, IArchiving<T>
        where T : IArchiveable, new()
    {
        private readonly IArchiving<T> _owner;
        private readonly ExceptionHandlingService _exceptionHandler;

        public ArchivingComponent(
            IArchiving<T> owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner)); 

            _exceptionHandler = new ExceptionHandlingService();

            ArchiveCmd = new RelayCommand(OnArchive, () => CanArchive);

            RestoreArchivedCmd = new RelayCommand(
                OnRestoreArchived, 
                () => CanExecuteRestoreArchived);
        }

        public ICommand ArchiveCmd { get; }

        public ICommand RestoreArchivedCmd { get; }

        public bool CanArchive => ViewState.IsNotBusy &&
            !ModelObject.IsNullOrNew() && ModelObject?.IsArchived == false;

        public bool CanExecuteRestoreArchived => ViewState.IsNotBusy &&
            !ModelObject.IsNullOrNew() && ModelObject?.IsArchived == true;

        public bool IsArchived { get; }

        public T ModelObject
        {
            get => _owner.ModelObject;
            set => _owner.ModelObject = value;
        }

        public IObservableModel Parent => _owner.Parent;

        public IViewStateViewModel ViewState => _owner.ViewState;

        public IModelService ModelService => _owner.ModelService;

        public IFocusable Owner
        {
            get => _owner.Owner;
            set => _owner.Owner = value;
        }

        public bool HasFocus
        {
            get => _owner.HasFocus;
            set => _owner.HasFocus = value;
        }

        public void CloseView(bool result = true)
        {
            _owner.CloseView(result);
        }

        private bool ConfirmArchive()
        {
            var value = false;            
            var confirmation = MessageBox.Show(
                "Are you sure? ",
                "Confirm Archive",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            if (confirmation == MessageBoxResult.Yes)
            {
                value = true;
            }
            return value;
        }

        private async void OnArchive()
        {
            MessengerInstance.Send(new BusyMessage(true, this));
            try
            {
                var confirmation = ConfirmArchive();
                if (confirmation)
                {
                    var old = ModelObject.Clone();

                    try
                    {
                        await ModelService.ArchiveAsync(
                            AuthUser.Current, ModelObject);
                        ModelObject.archived_at = ModelObject.updated_at =
                            DateTime.Now;
                    }
                    catch (ApiValidationException ex)
                    {
                        _exceptionHandler.ReactToException(this, ex);
                        return;
                    }
                    // Force reload
                    ModelObject.IsHeader = true;

                    MessengerInstance.Send(new ModelObjectPersistedMessage<T>(
                        _owner as IOwnedFocusable,
                        Owner as IOwnedFocusable,
                        old,
                        ModelObject,
                        Parent));
                }
            }
            finally
            {
                MessengerInstance.Send(new BusyMessage(false, this));
            }
            CloseView(true);
        }

        private async void OnRestoreArchived()
        {
            MessengerInstance.Send(new BusyMessage(true, this));
            try
            {
                var old = ModelObject.Clone();

                ModelObject.SetToRestore();

                await ModelService.UpdateAsync(
                    AuthUser.Current, _owner.ModelObject);
                ModelObject.updated_at = DateTime.Now;

                // Force reload
                ModelObject.IsHeader = true;

                MessengerInstance.Send(new ModelObjectPersistedMessage<T>(
                        _owner as IOwnedFocusable,
                        Owner as IOwnedFocusable,
                        old,
                        ModelObject,
                        Parent));
            }
            finally
            {
                MessengerInstance.Send(new BusyMessage(false, this));
            }
            CloseView(true);
        }

    }
}
