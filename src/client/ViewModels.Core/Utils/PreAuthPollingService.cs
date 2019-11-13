using savaged.mvvm.ViewModels.Core.Utils.Messages;
using GalaSoft.MvvmLight.Messaging;
using System;

namespace savaged.mvvm.ViewModels.Core.Utils
{
    public class PreAuthPollingService 
        : PollingService<PreAuthPollingDelayElapsedMessage>
    {
        public PreAuthPollingService(IMessenger messenger)
            : base(messenger)
        {
            Started = DateTime.MinValue;
        }

        public override void Start()
        {
            base.Start();
            Started = DateTime.Now;
        }

        public DateTime Started { get; private set; }

        protected override void ReactToPollingTimerElapsed(
            PreAuthPollingDelayElapsedMessage m)
        {
            m.SinceStarted = SignalTime.Subtract(Started);
            base.ReactToPollingTimerElapsed(m);
        }
    }
}
