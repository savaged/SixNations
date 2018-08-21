using System;

namespace SixNations.Exceptions
{
    public class AuthException : Exception
    {
        private const string defaultMsg = "Authentification failure";

        public AuthException() : base(defaultMsg)
        {
        }

        public AuthException(string msg) : base(msg)
        {
        }

        public AuthException(Exception innerException) : base(defaultMsg, innerException)
        {
        }
    }
}
