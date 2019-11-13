using savaged.mvvm.Navigation;
using GalaSoft.MvvmLight.Messaging;

namespace savaged.mvvm.ViewModels.Core.Utils.Messages
{
    public class ReloadingMessage : MessageBase
    {
        public ReloadingMessage(
            IOwnedFocusable sender)
        {
            Sender = sender;
        }
    }
}
