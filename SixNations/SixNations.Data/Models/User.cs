using GalaSoft.MvvmLight;

namespace SixNations.Data.Models
{
    public sealed class User : ObservableObject
    {
        #region threadsafe singleton

        private static volatile User _instance;
        private static readonly object ThreadSaftyLock = new object();

        static User() { }

        private User() { }

        public static User Current
        {
            get
            {
                if (_instance is null)
                {
                    lock (ThreadSaftyLock)
                    {
                        if (_instance is null)
                        {
                            _instance = new User();
                        }
                    }
                }
                return _instance;
            }
        }

        #endregion

        #region initialisation & state

        public void Initialise(string authToken)
        {
            AuthToken = authToken;
            RaisePropertyChanged(() => AuthToken);
            RaisePropertyChanged(() => IsLoggedIn);
        }

        public bool IsLoggedIn => !string.IsNullOrEmpty(AuthToken);

        public string AuthToken { get; private set; }

        #endregion
    }
}
