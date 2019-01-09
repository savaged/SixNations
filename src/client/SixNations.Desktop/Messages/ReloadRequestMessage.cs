using GalaSoft.MvvmLight.Messaging;

namespace SixNations.Desktop.Messages
{
    public class ReloadRequestMessage : MessageBase
    {
        public ReloadRequestMessage(object sender)
        {
            Sender = sender;
        }
    }
}
