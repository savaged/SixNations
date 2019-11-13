using savaged.mvvm.Core.Interfaces;
using GalaSoft.MvvmLight.Messaging;

namespace savaged.mvvm.ViewModels.Core.Utils.Messages
{
    public class ModelObjectSelectedMessage<T> : MessageBase 
        where T : IObservableModel, new()
    {
        public ModelObjectSelectedMessage(IViewModel sender, T selected) 
            : base(sender)
        {
            Selected = selected;
        }

        public T Selected { get; }
    }
}
