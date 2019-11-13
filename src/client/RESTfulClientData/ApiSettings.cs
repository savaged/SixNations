namespace savaged.mvvm.Data
{
    public class ApiSettings
    {
        public ApiSettings(
            bool isDefault = false, 
            string expectedApiVersionNumber = "")
        {
            if (isDefault)
            {
                RequestHeaderSettingName1 = "Accept";
                RequestHeaderSettingValue1 = "application/json";
                RequestHeaderSettingName2 = "Authorization";
                RequestHeaderSettingValue2 = "Bearer ";

                UploadedFileKey = "UploadedFile";
            }
            ExpectedApiVersion = expectedApiVersionNumber;
        }

        public string RequestHeaderSettingName1 { get; set; }

        public string RequestHeaderSettingValue1 { get; set; }

        public string RequestHeaderSettingName2 { get; set; }

        public string RequestHeaderSettingValue2 { get; set; }

        public bool? IsIndexExplicitlyPlural { get; set; }

        public string ExpectedApiVersion { get; set; }

        public string UploadedFileKey { get; set; }
    }
}