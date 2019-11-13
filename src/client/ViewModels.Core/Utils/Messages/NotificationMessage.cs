using savaged.mvvm.Core.Interfaces;
using GalaSoft.MvvmLight.Messaging;

namespace savaged.mvvm.ViewModels.Core.Utils.Messages
{
    public class NotificationMessage<T> : MessageBase 
        where T : IObservableModel
    {
        public NotificationMessage(IViewModel sender, string content)
        {
            Sender = sender;
            Content = content;
        }

        public string Content { get; private set; }
    }
}
