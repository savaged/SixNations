using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace savaged.mvvm.Data
{
    public class DataServiceGateway : IDataServiceGateway
    {
        private static readonly ILog _log = LogManager.GetLogger(
            MethodBase.GetCurrentMethod().DeclaringType);

        private readonly int[] _allowedStatusCodes;

        private readonly IApiFormatConverter _apiFormatConverter;

        public DataServiceGateway(
            string baseUrl,
            ApiSettings apiSettings,
            IApiFormatConverter apiFormatConverter = null)
        {
            _allowedStatusCodes = new int[] {
                200,
                503,
                401,
                403,
                405,
                409,
                410,
                412,
                422,
                423,
                429,
                404,
                426,
                418
            };
            BaseUrl = baseUrl;
            ApiSettings = apiSettings;
            _apiFormatConverter = apiFormatConverter;
        }

        public string BaseUrl { get; }

        public ApiSettings ApiSettings { get; }

        public async Task<IResponseFileStream> GetFileAsync(
            IAuthUser user, string uri)
        {
            HttpClient client = SetupHttpClient(user?.Token);

            _log.Debug($"Request for file stream on {uri}");

            HttpResponseMessage rawResponse = 
                await GetRawResponseAsync(
                client, HttpMethods.Get, uri, null, null);

            if (rawResponse == null) throw new ApiDataException(
                "Expected a response but got none.", user);

            var statusCode = (int)rawResponse.StatusCode;

            UnexpectedStatusCodeCheck(statusCode, rawResponse);

            if (rawResponse.Content == null)
            {
                throw new ApiDataException(
                    $"Got a response with status code {statusCode}, " +
                    $"but content is missing.",
                    user);
            }
            ResponseFileStream responseFileStream;
            var stream = await rawResponse.Content.ReadAsStreamAsync();

            if (rawResponse.Content.Headers == null)
            {
                throw new ApiDataException(
                    "Got a response with content and status code " +
                    $"{statusCode}, but headers are missing.",
                    user);
            }
            if (rawResponse.Content.Headers.ContentDisposition == null)
            {
                throw new ApiDataException(
                    $"Got a response with content," +
                    $" headers and status code {statusCode}," +
                    " but ContentDisposition is missing.",
                    user);
            }
            if (string.IsNullOrEmpty(rawResponse.Content.Headers
                .ContentDisposition.FileName))
            {
                throw new ApiDataException(
                    "Got a response with content," +
                    " headers, ContentDisposition and " +
                    $"status code {statusCode}, " +
                    "but FileName is missing.",
                    user);
            }
            AnticipatedStatusCodeErrorCheck(
                uri, 
                statusCode, 
                rawResponse.ReasonPhrase, 
                nameof(ResponseFileStream), 
                user);

            string filename = rawResponse.Content.Headers
                .ContentDisposition.FileName.ToString()
                .Replace("/", "").Replace("\"", "");

            responseFileStream = new ResponseFileStream(
                filename, stream);
            return responseFileStream;
        }


        public async Task<string> GetObjectAsync(
            IAuthUser user, string uri)
        {
            var value = await DataAsync(
                user, uri, HttpMethods.Get, null);
            return value;
        }

        public async Task<string> GetIndexAsync(
            IAuthUser user,
            string uri,
            IDictionary<string, object> data = null)
        {
            var httpMethod = HttpMethods.Get;
            if (data != null && data.Count > 0)
            {
                httpMethod = HttpMethods.Post;
            }
            var (Content, StatusCode, StatusReason) =
                await GetResponseContentAndStatusAsync(
                uri, httpMethod, data, user, _apiFormatConverter);

            _log.Debug(
                $"Response from {httpMethod.ToString().ToUpper()}: " +
                $"{uri}({StatusCode})->{Content}");

            AnticipatedStatusCodeErrorCheck(
                uri, StatusCode, StatusReason, Content, user);

            return Content;
        }

        public async Task<string> DataAsync(
            IAuthUser user,
            string uri, 
            HttpMethods httpMethod, 
            IDictionary<string, object> data = null)
        {
            var (Content, StatusCode, StatusReason) = 
                await GetResponseContentAndStatusAsync(
                uri, httpMethod, data, user, _apiFormatConverter);

            _log.Debug($"Response ({StatusCode}) from {httpMethod}: " +
                $"{uri}->{Content}");

            AnticipatedStatusCodeErrorCheck(
                uri, StatusCode, StatusReason, Content, user);

            return Content;
        }


        private HttpClient SetupHttpClient(string token)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation(
                ApiSettings?.RequestHeaderSettingName1,
                ApiSettings?.RequestHeaderSettingValue1);
            client.DefaultRequestHeaders.TryAddWithoutValidation(
                ApiSettings?.RequestHeaderSettingName2,
                $"{ApiSettings?.RequestHeaderSettingValue2}{token}");
            client.BaseAddress = new Uri(BaseUrl);
            return client;
        }

        private async Task<(string Content, int StatusCode, string StatusReason)> GetResponseContentAndStatusAsync(
            string uri,
            HttpMethods httpMethod,
            IDictionary<string, object> data,
            IAuthUser user,
            IApiFormatConverter apiFormatConverter = null)
        {
            if (data == null)
            {
                data = new Dictionary<string, object>();
            }
            var rawResponse = await HttpRequestRawResponseAsync(
                uri, 
                user, 
                httpMethod,
                data,
                apiFormatConverter);

            var rawContent = await GatewayHelper
                .ReadResponseContentAsync(rawResponse, user);

            var value = (rawContent, 
                (int)rawResponse.StatusCode, 
                rawResponse.ReasonPhrase);
            return value;
        }

        protected async Task<HttpResponseMessage> HttpRequestRawResponseAsync(
            string uri, 
            IAuthUser user, 
            HttpMethods httpMethod,
            IDictionary<string, object> data,
            IApiFormatConverter apiFormatConverter = null)
        {
            MultipartFormDataContent formData = null;

            AddApiVersionArg(ref data);

            if (ApiSettings?.UploadedFileKey != null
                && data.ContainsKey(ApiSettings.UploadedFileKey))
            {
                formData = GatewayHelper.FormatFormRequestContent(
                        data, ApiSettings.UploadedFileKey);
            }
            var httpContent = GatewayHelper.FormatHttpRequestContent(
                data, 
                ApiSettings?.RequestHeaderSettingValue1, 
                apiFormatConverter);

            var client = SetupHttpClient(user?.Token);

            _log.Info($"{httpMethod}: {BaseUrl}{uri}");
            if (data?.Count > 0)
            {
                var requestContent = await httpContent
                    .ReadAsStringAsync();

                _log.DebugFormat("\twith request content: {0}", 
                    requestContent);

                if (formData != null)
                {
                    _log.DebugFormat(
                        "\t and with request form data: {0}",
                        formData.ToString());
                }
            }
            HttpResponseMessage rawResponse = null;
            try
            {
                rawResponse = await GetRawResponseAsync(
                    client, httpMethod, uri, httpContent, formData);
            }
            catch (Exception ex)
            {
                _log.Error(
                    $"Unexpected error accessing gateway. \n{ex}");
                throw;
            }
            var statusCode = (int)rawResponse.StatusCode;

            UnexpectedStatusCodeCheck(statusCode, rawResponse);
            
            return rawResponse;
        }

        private void AddApiVersionArg(
            ref IDictionary<string, object> data)
        {
            const string key = "APIVersion";
            if (data is null)
            {
                data = new Dictionary<string, object>();
            }
            if (!data.Keys.Contains(key)
                && !string.IsNullOrEmpty(ApiSettings?.ExpectedApiVersion))
            {
                data.Add(key, ApiSettings?.ExpectedApiVersion);
            }
        }

        private void UnexpectedStatusCodeCheck(
            int statusCode, HttpResponseMessage rawResponse)
        {
            if (statusCode == 500 
                || !_allowedStatusCodes.Contains(statusCode))
            {
                // It's a 500 or a code we don't expect, we have no error to return, so throw a generic error.
                var msg = string.Format(
                    "Status Code: {0}, Error: {1}.",
                    statusCode, rawResponse.ReasonPhrase);
                throw new GatewayException(statusCode, msg);
            }
        }
        protected void AnticipatedStatusCodeErrorCheck(
            string url, 
            int statusCode, 
            string reason,
            string content,
            IAuthUser user)
        {
            if (_allowedStatusCodes.Contains(statusCode)
                && statusCode != 200
                && statusCode != 418)
            {
                switch (statusCode)
                {
                    case 401:
                    case 429:
                        var ex = new ApiAuthException(reason, user);
                        user?.ReactToException(ex);
                        throw ex;
                    case 403:
                        throw new ApiPermissionException(reason, user);
                    case 410:
                    case 412:
                    case 422:
                        throw new ApiValidationException(
                            statusCode, reason, content, user);
                    case 423:
                        throw new ApiRecordLockedException(
                            reason, user);
                    case 409:
                        throw new ApiRecordStateConflict(reason, user);
                    case 404:
                    case 405:
                        throw new ApiResourceNotFoundException(
                            url, reason, user);
                    case 426:
                        throw new ApiVersionException(reason, user);
                    case 503:
                        throw new MaintenanceModeException(
                            reason, user);
                    default:
                        throw new GatewayException(
                            statusCode, reason, user);
                }
            }
        }

        private async Task<HttpResponseMessage> GetRawResponseAsync(
            HttpClient client, 
            HttpMethods httpMethod, 
            string url, 
            StringContent httpContent = null, 
            MultipartFormDataContent formContent = null)
        {
            HttpResponseMessage rawResponse = null;
            try
            {
                switch (httpMethod)
                {
                    case HttpMethods.Post:
                        rawResponse = await client.PostAsync(
                            url, 
                            (formContent != null) ?
                            (HttpContent)formContent :
                            httpContent);
                        break;
                    case HttpMethods.Put:
                        rawResponse = await client.PutAsync(
                            url, httpContent);
                        break;
                    case HttpMethods.Delete:
                        rawResponse = await client.DeleteAsync(url);
                        break;
                    default:
                        rawResponse = await client.GetAsync(url);
                        break;
                }
            }
            catch (HttpRequestException ex)
            {
                throw new ApiUnavailableException(ex);
            }
            catch (NullReferenceException ex)
            {
                _log.Fatal(
                    "Something wrong with the API that is returning " +
                    "a null that cannot be anticipated in the client! " +
                    ex);
                throw;
            }
            return rawResponse;
        }
    }
}
