using System;

namespace savaged.mvvm.ViewModels.Core.Utils.Messages
{
    public class PreAuthPollingDelayElapsedMessage 
        : PollingDelayElapsedMessage
    {
        public TimeSpan SinceStarted { get; set; }
    }
}
