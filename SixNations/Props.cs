using System;

namespace SixNations
{
    static class Props
    {
#if DEBUG
        internal const string ApiBaseURL = "https://homestead.test/";
#else
        internal const string ApiBaseURL = "https://192.168.0.22/";
#endif
    }
}
