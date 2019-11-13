using GalaSoft.MvvmLight.Messaging;
using System;

namespace savaged.mvvm.ViewModels.Core.Utils.Messages
{
    public abstract class PollingDelayElapsedMessage : MessageBase 
    {
        public new object Sender { get; set; }
    }
}
