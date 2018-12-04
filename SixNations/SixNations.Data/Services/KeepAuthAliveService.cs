using System;
using System.Timers;
using System.Threading.Tasks;
using System.Collections.Generic;
using SixNations.Data.Models;
using SixNations.API.Helpers;
using SixNations.API.Constants;
using SixNations.API.Interfaces;

namespace SixNations.Data.Services
{
    public class KeepAuthAliveService : IKeepAuthAliveService
    {
        private readonly IHttpDataServiceFacade _httpDataServiceFacade;
        private readonly Timer _keepActivePollingTimer;
        private int _keepAliveCounter;
        private readonly int _keepAliveMaxCount;
        private const int _pollingDelayIncrementFactor = 165;

        public KeepAuthAliveService(IHttpDataServiceFacade httpDataServiceFacade)
        {
            _httpDataServiceFacade = httpDataServiceFacade;

            _keepAliveMaxCount = Props.POLLING_DELAY * 
                _pollingDelayIncrementFactor;

            _keepActivePollingTimer = new Timer(Props.POLLING_DELAY * 0.5)
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
                await GetModelDataAsync(User.Current.AuthToken, (ex) => { });
                _keepAliveCounter++;
            }
        }

        public async Task<IEnumerable<Tea>> GetModelDataAsync(
            string authToken, Action<Exception> exceptionHandler)
        {
            var uri = typeof(Tea).NameToUriFormat();
            IResponseRootObject response = null;
            try
            {
                response = await _httpDataServiceFacade
                    .HttpRequestAsync(uri, authToken);
            }
            catch (Exception ex)
            {
                exceptionHandler(ex);
                response = new ResponseRootObject(ex.Message);
            }
            var index = new ResponseRootObjectToModelMapper<Tea>(response).AllMapped();
            return index;
        }
    }
}