using System;

namespace SixNations.API.Constants
{
    /// <summary>
    /// The ApiBaseURL property is added to a partial that is
    /// not kept in source control so that the individual 
    /// developer setting does not have to change for all others.
    /// </summary>
    public static partial class Props
    {
        /// <summary>
        /// This can be used against the ApiBaseURL property to 
        /// switch to mocked data services.
        /// </summary>
        public const string MOCKED = "Mocked/";
    }
}
