using System;

namespace savaged.mvvm.Data
{
    public class AuthUser : IAuthUser
    {
        private static volatile IAuthUser _instance;
        private static readonly object _threadSaftyLock = new object();

        protected AuthUser(string email, string token)
        {
            Email = email;
            Token = token;
            Status = string.Empty;
        }

        public static IAuthUser Default(string email, string token)
        {
            if (!IsValidInstance)
            {
                lock (_threadSaftyLock)
                {
                    if (!IsValidInstance)
                    {
                        _instance = new AuthUser(email, token);
                    }
                }
            }
            return _instance;
        }

        private static bool IsValidInstance =>
            _instance != null && !string.IsNullOrEmpty(_instance.Token);

        public static IAuthUser Current =>
            Default(string.Empty, string.Empty);

        public static IAuthUser Null =>
            new AuthUser(string.Empty, string.Empty);

        public void Reset()
        {
            Token = Status = string.Empty;
        }

        public string Email { get; protected set; }

        public string Token { get; protected set; }

        public bool IsLoggedIn => !string.IsNullOrEmpty(Token);

        public override string ToString()
        {
            return Email;
        }

        public void ReactToException(ApiAuthException ex)
        {
            if (IsLoggedIn)
            {
                Reset();
                Status = ex.Message;
                LoggedOut?.Invoke(this, EventArgs.Empty);
            }
        }

        public string Status { get; private set; }

        public event EventHandler LoggedOut = delegate { };
    }

}