using GalaSoft.MvvmLight.Messaging;

namespace SixNations.Desktop.Messages
{
    public class CloseDialogRequestMessage : MessageBase
    {
        public CloseDialogRequestMessage(object sender, bool? dialogResult)
        {
            Sender = sender;
            DialogResult = dialogResult;
        }

        public bool? DialogResult { get; }
    }
}
