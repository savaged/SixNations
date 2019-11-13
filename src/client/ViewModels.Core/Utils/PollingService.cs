using savaged.mvvm.Core.Interfaces;
using savaged.mvvm.ViewModels.Core.Utils.Messages;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Timers;

namespace savaged.mvvm.ViewModels.Core.Utils
{
    public abstract class PollingService<T> : IPollingService
        where T : PollingDelayElapsedMessage, new()
    {
        private readonly IMessenger _messenger;
        private readonly int _pollingDelay;
        private readonly Timer _pollingTimer;

        public PollingService(IMessenger messenger, int pollingDelay = 0)
        {
            _messenger = messenger ??
                throw new ArgumentNullException(nameof(messenger));

            if (pollingDelay > 0)
            {
                _pollingDelay = pollingDelay;
            }
            else
            {
                _pollingDelay = Properties.Settings.Default.PollingDelay; 
            }

            _pollingTimer = new Timer(_pollingDelay)
            {
                AutoReset = true,
                Enabled = false
            };
            _pollingTimer.Elapsed += OnPollingTimerElapsed;
        }

        protected DateTime SignalTime { get; private set; }

        protected virtual void ReactToPollingTimerElapsed(T msg)
        {
            msg.Sender = this;
            _messenger.Send(msg);
        }

        private void OnPollingTimerElapsed(object sender, ElapsedEventArgs e)
        {
            SignalTime = e.SignalTime;
            ReactToPollingTimerElapsed(new T());
        }

        /// <summary>
        /// This stops the timer and removes the event
        /// </summary>
        public virtual void Kill()
        {
            Stop();
            _pollingTimer.Elapsed -= OnPollingTimerElapsed;
        }

        public virtual void Start()
        {
            _pollingTimer.Start();
        }

        public void Stop()
        {
            _pollingTimer.Stop();
        }
    }
}
