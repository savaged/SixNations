using System;

namespace SixNations.Desktop.StaticData
{
    static class Constants
    {
#if DEBUG
        internal const string ApiBaseURL = "http://homestead.test/";
#else
        internal const string ApiBaseURL = "http://192.168.0.22/";
#endif
    }
}
