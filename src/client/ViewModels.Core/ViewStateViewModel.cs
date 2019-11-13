using System;
using GalaSoft.MvvmLight;
using System.Collections.Generic;
using savaged.mvvm.ViewModels.Core.Utils.Messages;
using savaged.mvvm.Core.Interfaces;

namespace savaged.mvvm.ViewModels.Core
{
    public class ViewStateViewModel : ViewModelBase, IViewStateViewModel
    {
        private readonly IList<string> _registry;

        public ViewStateViewModel()
        {
            _registry = new List<string>();
            MessengerInstance.Register<BusyMessage>(this, OnBusyMessage);
        }

        public override void Cleanup()
        {
            MessengerInstance.Unregister<BusyMessage>(this);
        }

        public virtual bool IsBusy => _registry.Count > 0;

        public bool IsNotBusy => !IsBusy;

        public string DumpBusyRegistry()
        {
            var registry = "Registry: [";
            foreach (var caller in _registry)
            {
                registry += "{ caller: \"" + caller + "\" },";
            }
            registry += "]";
            return registry;
        }

        public IObservableModel Clipboard
        {
            get => _clipboard;
            set
            {
                if (value != null && value != _clipboard)
                {
                    value.Id = 0;
                    _clipboard = value;
                    System.Windows.Clipboard.SetText(
                        _clipboard.ToString());
                }
            }
        }
        private IObservableModel _clipboard;

        public bool CanPaste(Type T)
        {
            var value = Clipboard != null && Clipboard.GetType() == T;
            return value;
        }

        private void OnBusyMessage(IBusyMessage m)
        {
            if (m.IsBusy)
            {
                _registry.Add($"{m.CallerType}.{m.CallerMember}");
            }
            else
            {
                _registry.Remove($"{m.CallerType}.{m.CallerMember}");
            }
            RaisePropertyChanged(nameof(IsBusy));
            RaisePropertyChanged(nameof(IsNotBusy));
        }

        public void ResetBusyRegistry()
        {
            _registry.Clear();
            RaisePropertyChanged(nameof(IsBusy));
            RaisePropertyChanged(nameof(IsNotBusy));
        }
    }
}
