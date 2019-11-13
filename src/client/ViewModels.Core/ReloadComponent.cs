using savaged.mvvm.Core.Interfaces;
using savaged.mvvm.Navigation;
using savaged.mvvm.ViewModels.Core.Utils.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace savaged.mvvm.ViewModels.Core
{
    public class ReloadComponent : ObservableObject, IReloadable
    {
        private readonly IReloadable _owner;
        private IMessenger _messengerInstance;

        public ReloadComponent(
            IReloadable owner,
            IMessenger messengerInstance)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));

            ReloadCmd = new RelayCommand(OnReload, () => ViewState.IsNotBusy);

            _messengerInstance = messengerInstance ??
                throw new ArgumentNullException(nameof(messengerInstance));
            _messengerInstance.Register<RequestReloadMessage>(
                this, OnRequestReload);
        }

        public IViewStateViewModel ViewState => _owner.ViewState;

        public ICommand ReloadCmd { get; }

        public bool HasFocus
        {
            get => _owner.HasFocus;
            set => throw new NotSupportedException();
        }

        public IFocusable Owner
        {
            get => _owner;
            set => throw new NotSupportedException();
        }

        public async Task Reload()
        {
            if (_owner?.HasFocus == true)
            {
                _messengerInstance.Send(new ReloadingMessage(_owner));
                await _owner.Reload();
            }
        }

        private async void OnRequestReload(RequestReloadMessage m)
        {
            if (m.Target == _owner)
            {
                await Reload();
            }
        }
        private async void OnReload()
        {
            await Reload();
        }

    }
}
