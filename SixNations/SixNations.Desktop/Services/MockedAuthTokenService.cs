using log4net;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using SixNations.Desktop.Exceptions;
using SixNations.Desktop.Models;
using SixNations.Desktop.Facade;
using SixNations.Desktop.Interfaces;
using Moq;

namespace SixNations.Desktop.Services
{
    partial class MockedAuthTokenService : IAuthTokenService
    {
        private readonly string url;

        private static readonly ILog Log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public const string MOCK_TOKEN = "mockToken";

        private Mock<IAuthTokenService> MockAuthTokenService { get; }

        public MockedAuthTokenService()
        {
            url = $"{Constants.Props.ApiBaseURL}api/login";

            MockAuthTokenService = new Mock<IAuthTokenService>();
            MockAuthTokenService.Setup(s => s.GetTokenAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("mockToken");
        }

        public async Task<string> GetTokenAsync(string email, string password)
        {
            Log.Info($"Requesting access from {url}");

            var accessToken = await MockAuthTokenService.Object.GetTokenAsync("any", "any");

            Log.Info("Access token obtained");
            return accessToken;
        }
    }
}
