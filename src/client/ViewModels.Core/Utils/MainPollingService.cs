using savaged.mvvm.ViewModels.Core.Utils.Messages;
using GalaSoft.MvvmLight.Messaging;

namespace savaged.mvvm.ViewModels.Core.Utils
{
    public class MainPollingService 
        : PollingService<MainPollingDelayElapsedMessage>
    {
        public MainPollingService(IMessenger messenger)
            : base(messenger) { }
    }
}
