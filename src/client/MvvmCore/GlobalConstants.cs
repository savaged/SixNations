using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace savaged.mvvm.Core
{
    public partial class GlobalConstants
    {
        public const string APPLICATION_NAME = "AppName";

        public const string API_VERSION = "2.1.0";

        //public static string APPLICATION_FOLDER => $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\Downloads\\{APPLICATION_NAME}\\"; 
        public static string APPLICATION_FOLDER => $"D:\\Users\\{Environment.UserName}\\Downloads\\{APPLICATION_NAME}\\";

        public static string APPLICATION_TEMP_FOLDER => $"{Path.GetTempPath()}{APPLICATION_NAME}\\";

        public const string HELP_PAGES_LOCAION = "https://wiki.appname.co.uk/OperationsManual:";

        public static JsonSerializerSettings JsonSerializerSettings =
            new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore                
            };

    }
}
