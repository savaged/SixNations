using System;
using System.Timers;
using SixNations.API.Constants;
using SixNations.API.Interfaces;

namespace SixNations.Desktop.Services
{
    public class PollingService : IPollingService
    {
        #region threadsafe singleton

        private static volatile PollingService _instance;
        private static readonly object ThreadSaftyLock = new object();

        static PollingService() { }

        private PollingService()
        {
            _pollingTimer = new Timer(Props.POLLING_DELAY)
            {
                AutoReset = true,
                Enabled = false
            };
            _pollingTimer.Elapsed += OnPollingTimerElapsed;
        }

        public static PollingService Instance
        {
            get
            {
                if (_instance is null)
                {
                    lock (ThreadSaftyLock)
                    {
                        if (_instance is null)
                        {
                            _instance = new PollingService();
                        }
                    }
                }
                return _instance;
            }
        }

        #endregion

        private readonly Timer _pollingTimer;

        /// <summary>
        /// Requires restart of the service
        /// </summary>
        /// <param name="milliseconds"></param>
        public void SetInterval(int milliseconds)
        {
            Stop();
            _pollingTimer.Interval = milliseconds;
        }

        public void Kill()
        {
            Stop();
            _pollingTimer.Elapsed -= OnPollingTimerElapsed;
        }

        public void Start()
        {
            if (!_pollingTimer.Enabled)
            {
                _pollingTimer.Start();
            }
        }

        public void Stop()
        {
            _pollingTimer.Stop();
        }

        public event Action IntervalElapsed = delegate { };
        
        private void OnPollingTimerElapsed(object sender, ElapsedEventArgs e)
        {
            IntervalElapsed?.Invoke();
        }
    }
}
