using log4net;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace savaged.mvvm.Data
{
    [Serializable]
    public class MaintenanceModeException : GatewayException
    {
        public MaintenanceModeException()
            : this("", null) { }

        public MaintenanceModeException(string msg)
            : this(msg, null) { }

        public MaintenanceModeException(string msg, IAuthUser user)
            : base(503, $"API is set in maintenance mode. {msg}", user) { }
    }

    [Serializable]
    public class ApiVersionException : GatewayException
    {
        public ApiVersionException(string msg)
            : this(msg, null) { }

        public ApiVersionException(string msg, IAuthUser user)
            : base(426, msg, user) { }
    }

    [Serializable]
    public class ApiAuthException : GatewayException
    {
        private static readonly string _msgPrefix =
            "Authentication failure!";

        /// <summary>
        /// Needed for testing
        /// </summary>
        public ApiAuthException() : this("") { }

        public ApiAuthException(string msg)
            : base(401, $"{_msgPrefix} {msg}") { }

        public ApiAuthException(string msg, IAuthUser user)
            : base(401, $"{_msgPrefix} {msg}", user) { }

        public ApiAuthException(
            Exception innerException, IAuthUser user)
            : base(401, _msgPrefix, innerException, user) { }

        protected ApiAuthException(
            int statusCode, string msg, IAuthUser user)
            : base(statusCode, msg, user) { }
    }
    [Serializable]
    public class TooManyLoginAttemptsException : ApiAuthException
    {
        public TooManyLoginAttemptsException(IAuthUser user)
            : base(429,
                  "Too many login attempts. Please try again later.",
                  user)
        { }
    }

    [Serializable]
    public class ApiPermissionException : GatewayException
    {
        public ApiPermissionException(IAuthUser user)
            : base(403, "API action not permitted", user) { }

        public ApiPermissionException(string msg, IAuthUser user)
            : base(403, msg, user) { }
    }
    [Serializable]
    public class ApiValidationException : GatewayException
    {
        public ApiValidationException(
            int statusCode, string reason, string msg)
            : this(statusCode, reason, msg, null) { }

        public ApiValidationException(
            int statusCode, string reason, string msg, IAuthUser user)
            : base(statusCode, $"{reason}. {msg}", user) { }
    }

    [Serializable]
    public class ApiRecordLockedException : GatewayException
    {
        public ApiRecordLockedException(string error)
            : this(error, null) { }

        public ApiRecordLockedException(string error, IAuthUser user)
            : base(423, error, user) { }
    }

    [Serializable]
    public class ApiRecordStateConflict : GatewayException
    {
        public ApiRecordStateConflict(string error)
            : this(error, null) { }

        public ApiRecordStateConflict(string error, IAuthUser user)
            : base(409, error, user) { }
    }

    [Serializable]
    public class ApiUnavailableException : GatewayException
    {
        private const string _msg = "API Server Unavailable!";

        public ApiUnavailableException(HttpRequestException innerException)
            : this(innerException, null) { }

        public ApiUnavailableException(IAuthUser user)
            : base(504, _msg, user) { }

        public ApiUnavailableException(
            HttpRequestException innerException, IAuthUser user)
            : base(504, _msg, innerException, user) { }
    }

    [Serializable]
    public class GatewayException : DesktopException
    {
        public GatewayException(int statusCode, string msg)
            : this(statusCode, msg, null) { }

        public GatewayException(
            int statusCode, string msg, IAuthUser user)
            : base(msg, user)
        {
            StatusCode = statusCode;
        }

        public GatewayException(
            int statusCode,
            string msg,
            Exception innerException,
            IAuthUser user)
            : base($"{msg}. Status code: {statusCode}.",
                  innerException,
                  user)
        {
            StatusCode = statusCode;
        }

        public int StatusCode { get; }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            info.AddValue(nameof(StatusCode), StatusCode);
            base.GetObjectData(info, context);
        }
    }

    [Serializable]
    public class ApiResourceNotFoundException : GatewayException
    {
        private const string _msg = "API resource not found!";

        public ApiResourceNotFoundException(string resource, string msg)
            : this(resource, msg, null) { }

        public ApiResourceNotFoundException(
            string resource, string msg, IAuthUser user)
            : base(404, $"{_msg} {resource}. {msg}", user) { }
    }


    [Serializable]
    public class ApiFileInputException : DesktopException
    {
        public ApiFileInputException(string fileLocation, IOException ex)
            : this(fileLocation, ex, null) { }

        public ApiFileInputException(string fileLocation, string msg)
            : this(fileLocation, msg, null) { }

        public ApiFileInputException(
            string fileLocation, string msg, IAuthUser user)
            : base($"Error reading '{fileLocation}'. {msg}", user) { }

        public ApiFileInputException(
            string fileLocation, IOException ioe, IAuthUser user)
            : base($"Error reading '{fileLocation}'", ioe, user) { }
    }

    [Serializable]
    public class ApiDataException : DesktopException
    {
        public ApiDataException(
            string msg, Exception innerException, IAuthUser user)
            : base(msg, innerException, user) { }

        public ApiDataException(string msg, IAuthUser user)
            : base(msg, user: user) { }

        public ApiDataException(Exception innerException)
            : this(innerException, null) { }

        public ApiDataException(string msg, Exception innerException)
            : this(msg, innerException, null) { }

        public ApiDataException(string msg)
            : base(msg) { }

        public ApiDataException(Exception innerException, IAuthUser user)
            : this("Unexpected data from API", innerException, user) { }
    }

    [Serializable]
    public class LookupConversionException : DesktopException
    {
        private const string _msgPrefix = "Error converting: ";

        public LookupConversionException(
            string responseContent,
            JsonSerializationException ex,
            IAuthUser user)
            : base($"{_msgPrefix}{responseContent}", ex, user) { }
    }


    [Serializable]
    public class DesktopException : Exception
    {
        private const string _msgPrefix = "Unexpected Error! ";

        private static readonly ILog _log = LogManager.GetLogger(
                MethodBase.GetCurrentMethod().DeclaringType);

        public DesktopException(string msg, IAuthUser user)
            : base(msg)
        {
            LogError(msg, null, null, user);
        }

        public DesktopException(string msg)
            : this(msg, user: null) { }

        public DesktopException(
            string msg, Exception innerException)
            : this(msg, innerException, null) { }

        public DesktopException(
            string msg, Exception innerException, IAuthUser user)
            : base(msg, innerException)
        {
            LogError($"{msg}. Inner: {innerException?.Message}",
                innerException?.StackTrace, innerException?.Source, user);
        }

        public DesktopException(Exception ex, IAuthUser user)
            : base(_msgPrefix, ex)
        {
            LogError(ex.Message, ex.StackTrace, ex.Source, user);
        }

        private void LogError(
            string msg, string stackTrace, string source, IAuthUser user)
        {
            var st = new StackTrace(true);
            if (string.IsNullOrEmpty(stackTrace))
            {
                stackTrace = st.ToString();
            }
            if (string.IsNullOrEmpty(source))
            {
                var sf = st.GetFrame(st.FrameCount - 1);
                if (sf != null)
                {
                    source = $"{sf.GetFileName()} Line " +
                        $"{sf.GetFileLineNumber()}";
                }
            }
            _log.Error($"{_msgPrefix} " +
                $"'{msg}'. " +
                $"Stack: {stackTrace}. " +
                $"Source: {source}. " +
                $"User: {user}");
        }
    }
}
