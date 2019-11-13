using savaged.mvvm.Core.Interfaces;
using GalaSoft.MvvmLight.Messaging;

namespace savaged.mvvm.ViewModels.Core.Utils.Messages
{
    public class SeedModelObjectRequestedMessage<T> : MessageBase
        where T : IObservableModel
    {
        public SeedModelObjectRequestedMessage(
            object sender,
            int modelObjectId)
            : base(sender)
        {
            ModelObjectId = modelObjectId;
        }

        public int ModelObjectId { get; }
    }
}
