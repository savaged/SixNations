using System;

namespace SixNations
{
    static class Props
    {
#if DEBUG
        internal const string ApiBaseURL = "http://homestead.app/";
#else
        internal const string ApiBaseURL = "http://192.168.0.22/";
#endif
    }
}
