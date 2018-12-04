using System;

namespace SixNations.API.Constants
{
    /// <summary>
    /// The ApiBaseURL property can be added to a partial that is
    /// not kept in source control so that the individual 
    /// developer setting does not have to change for all others.
    /// </summary>
    public static class Props
    {
        public const string MOCKED = "Mocked/";
#if DEBUG
        public static readonly string ApiBaseURL = "http://homestead.test/";//MOCKED;// 
#else
        public static readonly string ApiBaseURL = "https://verivi.co.uk/";
#endif
        public const int POLLING_DELAY = 120000;
    }
}
