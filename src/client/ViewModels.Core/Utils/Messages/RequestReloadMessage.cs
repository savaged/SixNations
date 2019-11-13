using savaged.mvvm.Navigation;
using GalaSoft.MvvmLight.Messaging;

namespace savaged.mvvm.ViewModels.Core.Utils.Messages
{
    public class RequestReloadMessage : MessageBase
    {
        public RequestReloadMessage(
            IOwnedFocusable sender,
            IOwnedFocusable target)
        {
            Sender = sender;
            Target = target;
        }
    }
}
