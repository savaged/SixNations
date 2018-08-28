using System;
using GalaSoft.MvvmLight.Messaging;

namespace SixNations.Desktop.Messages
{
    public class BusyMessage : MessageBase
    {
        public BusyMessage(bool isBusy)
        {
            IsBusy = isBusy;
        }

        public bool IsBusy { get; }
    }
}
