using log4net;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace savaged.mvvm.Data
{
    /// <summary>
    /// TODO Add a matching partial wity your client secret
    /// `private const string ClientSecret = "your client secret here";`
    /// </summary>
    public partial class AuthUserService : IAuthUserService
    {
        private readonly Uri _baseUrl;

        private static readonly ILog _log = LogManager.GetLogger(
            MethodBase.GetCurrentMethod().DeclaringType);

        public AuthUserService(string baseUrl)
        {
            _baseUrl = new Uri(baseUrl);
        }

        public async Task SetAuthUser(
            string email, string password, IAuthUser authUser)
        {
            var token = string.Empty;

            _log.Info($"Requesting access from {_baseUrl}");

            var client = new HttpClient
            {
                BaseAddress = _baseUrl
            };
            var preValues = GetPreValues(email, password);
            var preContent = new FormUrlEncodedContent(preValues);

            HttpResponseMessage response;
            try
            {
                response = await client.PostAsync("logon", preContent);
            }
            catch (Exception e)
            {
                // TODO Important! Catch the specific exception type(s) only
                throw new ApiAuthException(e, authUser);
            }

            var rawResponseContent = await GatewayHelper
                .ReadResponseContentAsync(response, authUser);

            var statusCode = (int)response.StatusCode;
            switch (statusCode)
            {
                case 429:
                    throw new TooManyLoginAttemptsException(authUser);
                case 423:
                    throw new ApiAuthException(
                        $"{response.ReasonPhrase}. {rawResponseContent}. " +
                        "Please try the web version at " +
                        "https://verivi.co.uk",
                        authUser);
                case 401:
                case 405:
                case 500:
                    throw new ApiAuthException(
                        $"{response.ReasonPhrase}. {rawResponseContent}. " +
                        "Please check with support.",
                        authUser);
            }

            token = rawResponseContent;

            if (string.IsNullOrEmpty(token))
            {
                throw new ApiAuthException(
                    "Unexpected exception meaning execution " +
                    "could not get access token!",
                    authUser);
            }
            _log.Info("Access token obtained");

            token = token.TrimIfQuoted('"');

            authUser = AuthUser.Default(email, token);
        }

        private Dictionary<string, string> GetPreValues(
            string username, string password)
        {
            var preValues = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "client_id", "1" },
                { "client_secret", ClientSecret },
                { "username", username },
                { "password", password },
                { "scope", "*" }
            };
            return preValues;
        }
    }
}