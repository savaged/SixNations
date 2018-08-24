using System;

namespace SixNations.Desktop.Exceptions
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
