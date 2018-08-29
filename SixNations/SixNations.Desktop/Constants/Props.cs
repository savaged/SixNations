using System;

namespace SixNations.Desktop.Constants
{
    static class Props
    {
        internal const string MOCKED = "Mocked/";
#if DEBUG
        /// <summary>
        /// Can use a Url or the word "Mocked/" for dummy services
        /// </summary>
        internal static readonly string ApiBaseURL = MOCKED; // "http://homestead.test/";
#else
        internal static readonly string ApiBaseURL = "http://192.168.0.22/";
#endif
    }
}
