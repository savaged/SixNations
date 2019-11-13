using savaged.mvvm.Core.Interfaces;
using log4net;
using System;
using System.Reflection;
using System.Timers;

namespace savaged.mvvm.ViewModels.Core.Utils
{
    public class KeepAlivePollingService : IPollingService
    {
        private static readonly ILog _log = 
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IKeepAlivePollingServiceClient _owner;
        private readonly Timer _keepActivePollingTimer;
        private readonly int _keepAliveMaxCount;
        private int _keepAliveCounter;

        public KeepAlivePollingService(
            IKeepAlivePollingServiceClient owner)
        {
            _owner = owner;

            _keepAliveMaxCount = Properties.Settings.Default.PollingMaxCount;

            _keepActivePollingTimer = new Timer(
                Properties.Settings.Default.PollingDelay * 6)
            {
                AutoReset = true,
                Enabled = false
            };
            _keepActivePollingTimer.Elapsed += OnKeepActivePollingTimerElapsed;
        }

        public void Start()
        {
            _keepActivePollingTimer.Start();
        }

        public void Stop()
        {
            _keepActivePollingTimer.Stop();
        }

        public void Kill()
        {
            Stop();
            _keepActivePollingTimer.Elapsed -= OnKeepActivePollingTimerElapsed;
        }

        private async void OnKeepActivePollingTimerElapsed(
            object sender, ElapsedEventArgs e)
        {
            if (_keepAliveCounter < _keepAliveMaxCount)
            {
                try
                {
                    await _owner?.ServiceCallToKeepAliveAsync();
                }
                catch (OperationCanceledException)
                {
                    _log.Warn(
                        "Service Call To Keep Alive cancelled for unknown reason!");
                }
                _keepAliveCounter++;
            }
        }

    }
}
