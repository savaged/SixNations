using System;

namespace savaged.mvvm.Data
{
    public interface IAuthUser 
    {
        string Email { get; }
        bool IsLoggedIn { get; }
        string Token { get; }
        void Reset();
        string ToString();
        string Status { get; }

        void ReactToException(ApiAuthException ex);

        event EventHandler LoggedOut;
    }
}