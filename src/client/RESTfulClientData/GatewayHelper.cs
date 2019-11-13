using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace savaged.mvvm.Data
{
    static class GatewayHelper
    {
        private static readonly ILog _log = LogManager.GetLogger(
            MethodBase.GetCurrentMethod().DeclaringType);
        
        public static async Task<string> ReadResponseContentAsync(
            HttpResponseMessage rawResponse, IAuthUser user)
        {
            string rawResponseContent;
            try
            {
                // TODO: to improve performance read to stream (see https://www.newtonsoft.com/json/help/html/Performance.htm)
                rawResponseContent = await rawResponse.Content
                    .ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _log.Error($"API read error {ex.Message}");
                throw;
            }
            if (string.IsNullOrEmpty(rawResponseContent))
            {
                _log.Warn("Empty raw response context from API");
            }
            return rawResponseContent;
        }

        public static StringContent FormatHttpRequestContent(
            IDictionary<string, object> data,
            string defaultRequestHeaderValue1,
            IApiFormatConverter apiFormatConverter = null)
        {
            if (data != null && apiFormatConverter != null)
            {
                data = apiFormatConverter.Convert(data);
            }
            var json = JsonConvert.SerializeObject(data);
            var httpContent = new StringContent(
                json, Encoding.UTF8, defaultRequestHeaderValue1);
            return httpContent;
        }

        public static MultipartFormDataContent FormatFormRequestContent(
            IDictionary<string, object> data, string uploadedFileKey)
        {
            var form = new MultipartFormDataContent();

            foreach (KeyValuePair<string, object> entry in data)
            {
                if (entry.Key == uploadedFileKey)
                {
                    byte[] content;
                    var fileLocation = entry.Value.ToString();
                    try
                    {
                        content = File.ReadAllBytes(fileLocation);
                    }
                    catch (IOException ex)
                    {
                        throw new ApiFileInputException(fileLocation, ex);
                    }
                    form.Add(
                        new ByteArrayContent(content), 
                        entry.Key, 
                        Path.GetFileName(entry.Value.ToString()));
                }
                else
                {
                    if (entry.Value != null)
                    {
                        form.Add(
                            new StringContent(entry.Value.ToString()), 
                            entry.Key);
                    }
                }
            }
            return form;
        }

    }
}
