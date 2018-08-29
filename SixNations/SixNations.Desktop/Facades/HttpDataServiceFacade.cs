using log4net;
using System;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using SixNations.Desktop.Interfaces;
using SixNations.Desktop.Models;
using SixNations.Desktop.Exceptions;

namespace SixNations.Desktop.Facade
{
    public class HttpDataServiceFacade : IHttpDataServiceFacade
    {
        private static readonly ILog Log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static int[] allowedStatusCodes = {
            200,
            503,
            401,
            410,
            412,
            422,
            423
        };

        private const string DefaultRequestHeaderName1 = "Accept";
        private const string DefaultRequestHeaderValue1 = "application/json";
        private const string DefaultRequestHeaderName2 = "Authorization";
        private const string DefaultRequestHeaderValue2 = "Bearer ";

        public async Task<ResponseRootObject> HttpRequestAsync(string uri, string token)
        {
            return await HttpRequestAsync(uri, token, HttpMethods.Get, null);
        }
        
        public async Task<ResponseRootObject> HttpRequestAsync(
            string uri, string token, HttpMethods httpMethod, IDictionary<string, object> data)
        {
            var url = GetUrl(uri);
            Log.Info($"Request initiated from {httpMethod}: {url}");

            var rawResponse = await HttpRequestRawResponseAsync(uri, token, httpMethod, data);
            var rawResponseContent = await ReadResponseContentAsync(rawResponse);
            var statusCode = (int)rawResponse.StatusCode;            

            var responseRootObject = DeserializeResponseRootObject(rawResponseContent, statusCode, url);

            // TODO on API change from using an Error property and put it in the response ReasonPhrase
            //      or the base class of all models
            //var statusCodeReason = rawResponse.ReasonPhrase;
            AnticipatedStatusCodeErrorCheck(statusCode, responseRootObject);

            EmulateEditLocking(url, httpMethod, ref responseRootObject);

            Log.Info($"Request complete from {httpMethod}: {url}");
            Log.Debug($"Response from {httpMethod}: {url}-> {responseRootObject.ToString()}");
            return responseRootObject;
        }

        private static StringContent FormatHttpRequestContent(IDictionary<string, object> data)
        {
            if (data != null)
            {
                data = TransformToApiExpectedFormat(data);
            }
            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            var httpContent = new StringContent(json, Encoding.UTF8, DefaultRequestHeaderValue1);
            return httpContent;
        }

        private static IDictionary<string, object> TransformToApiExpectedFormat(
            IDictionary<string, object> data)
        {
            var transformed = new Dictionary<string, object>();
            foreach (var kvp in data)
            {
                if (kvp.Value != null && kvp.Value.GetType() == typeof(DateTime))
                {
                    var dt = (DateTime)kvp.Value;
                    transformed[kvp.Key] = dt.ToString("yyyy-MM-dd");
                }
                else
                {
                    transformed[kvp.Key] = kvp.Value;
                }
            }
            return transformed;
        }

        private static MultipartFormDataContent FormatFormRequestContent(
            IDictionary<string, object> data)
        {
            var form = new MultipartFormDataContent();

            foreach (KeyValuePair<string, object> entry in data)
            {
                if (entry.Value != null)
                {
                    form.Add(new StringContent(entry.Value.ToString()), entry.Key);
                }
            }
            return form;
        }

        private static async Task<HttpResponseMessage> HttpRequestRawResponseAsync(
            string uri, string token, HttpMethods httpMethod, IDictionary<string, object> data)
        {
            MultipartFormDataContent formData = null;

            var httpContent = FormatHttpRequestContent(data);

            HttpClient client = SetupHttpClient(token);

            var url = GetUrl(uri);
            //var args = GetArgs(data);
            Log.Debug(httpMethod + ": " + url);
            if (data?.Count > 0)
            {
                var requestContent = await httpContent.ReadAsStringAsync();
                Log.DebugFormat("\twith request content: {0}", requestContent);
            }
            HttpResponseMessage rawResponse = null;
            try
            {
                rawResponse = await GetRawResponseAsync(client, httpMethod, url, httpContent, formData);
            }
            catch (Exception ex)
            {
                Log.Fatal($"Unexpected error accessing gateway. \n{ex.Message}");
                throw;
            }

            var statusCode = (int)rawResponse.StatusCode;

            UnexpectedStatusCodeCheck(statusCode, rawResponse);

            return rawResponse;
        }

        private static void UnexpectedStatusCodeCheck(int statusCode, HttpResponseMessage rawResponse)
        {
            if (statusCode == 500 || !allowedStatusCodes.Contains(statusCode))
            {
                // It's a 500 or a code we don't expect, we have no error to return, so throw a generic error.
                var msg = string.Format(
                    "Status Code: {0}, Error: {1}.", statusCode, rawResponse.ReasonPhrase);
                throw new HttpDataServiceException(statusCode, msg);
            }
        }
        private static void AnticipatedStatusCodeErrorCheck(
            int statusCode, ResponseRootObject responseRootObject)
        {
            if (allowedStatusCodes.Contains(statusCode) && statusCode != 200)
            {
                if (responseRootObject == null)
                {
                    throw new HttpDataServiceException(
                        statusCode, "Unexpected result: API with an allowed status code returned null!");
                }
                if (string.IsNullOrEmpty(responseRootObject.Error))
                {
                    throw new HttpDataServiceException(
                        statusCode,
                        "Unexpected result: API with an allowed status code returned" +
                        " response root object with null or empty Error property!");
                }
                throw new HttpDataServiceException(statusCode, responseRootObject.Error);
            }
        }

        private static string GetUrl(string uri)
        {
            if (uri.Contains("?"))
            {
                string[] words = uri.Split('?');
                uri = "";
                foreach (string part in words)
                {
                    uri += part.Contains('=') ? "?" + part : part.ToLower();
                }
            }
            var url = $"{Constants.Props.ApiBaseURL}api/{uri}";
            return url;
        }
        private static string GetArgs(IDictionary<string, object> data)
        {
            if (data == null || data.Count == 0)
            {
                return string.Empty;
            }
            var args = new StringBuilder("{\"Data\":[{");
            foreach (var kvp in data)
            {
                args.Append("\"");
                args.Append(kvp.Key);
                args.Append("\":");
                if (kvp.Value == null)
                {
                    args.Append("null");
                }
                else
                {
                    if (kvp.Value is string || kvp.Value is DateTime)
                    {
                        args.Append("\"");
                        args.Append(kvp.Value);
                        args.Append("\"");
                    }
                    else
                    {
                        args.Append(kvp.Value);
                    }
                }
                args.Append(",");
            }
            args.Append("]}");
            return args.ToString();
        }

        private static async Task<HttpResponseMessage> GetRawResponseAsync(
            HttpClient client, 
            HttpMethods httpMethod, 
            string url, 
            StringContent httpContent = null, 
            MultipartFormDataContent formContent = null)
        {
            HttpResponseMessage rawResponse = null;
            switch (httpMethod)
            {
                case HttpMethods.Post:
                    rawResponse = await client.PostAsync(url, (formContent != null) ?
                        (HttpContent)formContent : httpContent);
                    break;
                case HttpMethods.Put:
                    rawResponse = await client.PutAsync(url, httpContent);
                    break;
                case HttpMethods.Delete:
                    rawResponse = await client.DeleteAsync(url);
                    break;
                default:
                    rawResponse = await client.GetAsync(url);
                    break;
            }
            return rawResponse;
        }

        private static ResponseRootObject DeserializeResponseRootObject(
            string rawResponseContent, int statusCode, string url)
        {
            ResponseRootObject responseRootObject;
            try
            {
                responseRootObject =
                    JsonConvert.DeserializeObject<ResponseRootObject>(
                        rawResponseContent, new ResponseConverter());
            }
            catch (JsonReaderException ex)
            {
                var msg = $"The raw response could not be read by Json Reader: {ex.Message}." +
                    $" Raw: [{rawResponseContent}]";
                Log.Error(msg);
                responseRootObject = new ResponseRootObject(msg);
            }
            catch (JsonSerializationException ex)
            {
                var msg = "Deserialisation 'error' on response from: " +
                    $"{url}. NOTE FOR DEV: " +
                    "Check the API method is defined to return an array of " +
                    "objects in 'data' even if there is just one. However, " +
                    "this may simply be that no data was returned. This can " +
                    "happen with a call to an API Create method which has " +
                    "no templating. " +
                    $"Here is the JsonSerializationException: {ex.Message}";
                Log.Error(msg);
                responseRootObject = new ResponseRootObject(msg);
            }
            if (responseRootObject == null)
            {
                throw new Exception(
                    $"Expected the {nameof(rawResponseContent)} to convert but it has returned null! " +
                    $"Contents: [{rawResponseContent}]");
            }
            return responseRootObject;
        }

        internal static async Task<string> ReadResponseContentAsync(HttpResponseMessage rawResponse)
        {
            string rawResponseContent;
            try
            {
                // TODO: to improve performance read to stream (see https://www.newtonsoft.com/json/help/html/Performance.htm)
                rawResponseContent = await rawResponse.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Log.Error($"API read error {ex.Message}");
                throw;
            }
            if (string.IsNullOrEmpty(rawResponseContent))
            {
                throw new Exception("Empty raw response context from API!");
            }
            return rawResponseContent;
        }

        private static HttpClient SetupHttpClient(string token)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation(
                DefaultRequestHeaderName1, DefaultRequestHeaderValue1);
            client.DefaultRequestHeaders.TryAddWithoutValidation(
                DefaultRequestHeaderName2, DefaultRequestHeaderValue2 + token);
            return client;
        }

        /// <summary>
        /// We can move this into the API if we feel it is needed.
        /// The ResponseRootObject argument passed in with its editable flag set.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="httpMethod"></param>
        /// <param name="responseRootObject"></param>
        /// <returns></returns>
        private static void EmulateEditLocking(
            string url, HttpMethods httpMethod, ref ResponseRootObject responseRootObject)
        {
            if (responseRootObject == null)
            {
                throw new ArgumentNullException(nameof(responseRootObject));
            }
            if (httpMethod == HttpMethods.Get && (url.EndsWith("edit") || url.Contains("create")))
            {
                responseRootObject.__SetIsLockedForEditing();
            }
        }
    }
}