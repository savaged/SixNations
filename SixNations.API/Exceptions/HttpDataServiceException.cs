using System;

namespace SixNations.API.Exceptions
{
    [Serializable]
    public class HttpDataServiceException : Exception
    {
        public HttpDataServiceException(int statusCode, string msg) : base(msg)
        {
        }

        public HttpDataServiceException(int statusCode, string msg, Exception innerException) 
            : base(msg, innerException)
        {
        }
    }
}
