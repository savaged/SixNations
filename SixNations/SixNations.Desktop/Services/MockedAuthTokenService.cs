using log4net;
using System.Threading.Tasks;
using SixNations.Desktop.Interfaces;

namespace SixNations.Desktop.Services
{
    partial class MockedAuthTokenService : IAuthTokenService
    {
        private readonly string url;

        private static readonly ILog Log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        public MockedAuthTokenService()
        {
            url = $"{API.Constants.Props.ApiBaseURL}api/login";
        }

        public async Task<string> GetTokenAsync(string email, string password)
        {
            Log.Info($"Requesting access from {url}");
            await Task.CompletedTask;
            Log.Info("Access token obtained");
            return "mockToken";
        }
    }
}
