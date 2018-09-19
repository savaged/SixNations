using System;

namespace SixNations.API.Exceptions
{
    [Serializable]
    public class AuthServiceException : Exception
    {
        private const string defaultMsg = "Authentification failure";

        public AuthServiceException() : base(defaultMsg)
        {
        }

        public AuthServiceException(string msg) : base(msg)
        {
        }

        public AuthServiceException(Exception innerException) : base(defaultMsg, innerException)
        {
        }
    }
}
