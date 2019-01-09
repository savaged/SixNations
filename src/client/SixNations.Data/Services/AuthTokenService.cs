using log4net;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using SixNations.API.Interfaces;
using SixNations.API.Exceptions;
using SixNations.Data.Models;
using SixNations.Data.Facade;

namespace SixNations.Data.Services
{
    /// <summary>
    /// The ClientSecret property is added to a partial because this
    /// should not be included in the source control repository.
    /// </summary>
    public partial class AuthTokenService : IAuthTokenService
    {
        private readonly string url;

        private static readonly ILog Log = 
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public AuthTokenService()
        {
            url = $"{API.Constants.Props.ApiBaseURL}api/login";
        }

        public async Task<string> GetTokenAsync(string email, string password)
        {
            Log.Info($"Requesting access from {url}");

            string accessToken;
            var client = new HttpClient();

            var preValues = GetPreValues(email, password);

            var preContent = new FormUrlEncodedContent(preValues);

            HttpResponseMessage response;
            try
            {
                response = await client.PostAsync(url, preContent);
            }
            catch (Exception e)
            {
                throw new AuthServiceException(e);
            }

            var statusCode = (int)response.StatusCode;
            if (statusCode == 429)
            {
                throw new AuthServiceException("Too many login attempts. Please try again later.");
            }

            var rawResponseContent = await HttpDataServiceFacade.ReadResponseContentAsync(response);
            var responseRootObject = DeserializeResponseRootObject(rawResponseContent, url);

            // If no access token then throw exception
            if (responseRootObject.Error != null && responseRootObject.Error != "")
            {
                throw new AuthServiceException(responseRootObject.Error);
            }

            accessToken = (string)responseRootObject.Data[0]["access_token"];
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new AuthServiceException(
                    "Unexpected exception meaning execution could not get access token!");
            }
            Log.Info("Access token obtained");
            return accessToken;
        }

        private Dictionary<string, string> GetPreValues(string username, string password)
        {
            var preValues = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "client_id", "1" },
                { "client_secret", ClientSecret },
                { "username", username },
                { "password", password },
                { "scope", "*" },
                { "ip", GetIPAddress() }
            };
            return preValues;
        }

        private ResponseRootObject DeserializeResponseRootObject(string rawResponseContent, string url)
        {
            ResponseRootObject responseRootObject;
            try
            {
                responseRootObject =
                    JsonConvert.DeserializeObject<ResponseRootObject>(rawResponseContent, new ResponseConverter());
            }
            catch (JsonReaderException)
            {
                Log.Error(
                    $"The raw response could not be read by Json Reader. Raw: [{rawResponseContent}]");
                responseRootObject = new ResponseRootObject(rawResponseContent)
                {
                    Success = true
                };
            }
            catch (JsonSerializationException ex)
            {
                var msg = "Deserialisation 'error' on response from: " +
                    url + ". NOTE FOR DEV: " +
                    "Check the API method is defined to return an array of " +
                    "objects in 'data' even if there is just one. However, " +
                    "this may simply be that no data was returned. This can " +
                    "happen with a call to an API Create method which has " +
                    "no templating. " +
                    "Here is the JsonSerializationException: " + ex.Message;
                Log.Error(msg);
                responseRootObject = new ResponseRootObject(msg);
            }
            return responseRootObject;
        }

        private string GetIPAddress()
        {
            var value = string.Empty;
            try
            {
                var Host = default(IPHostEntry);
                string Hostname = null;
                Hostname = Environment.MachineName;
                Host = Dns.GetHostEntry(Hostname);
                foreach (var ip in Host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        value = Convert.ToString(ip);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"Failed to get user's IP address - {e.Message}");
            }
            return value;
        }
    }
}
