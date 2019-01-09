using System;
using GalaSoft.MvvmLight.Messaging;

namespace SixNations.Desktop.Messages
{
    public class AuthenticatedMessage : MessageBase
    {
        public AuthenticatedMessage(string token)
        {
            Token = token;
        }

        public string Token { get; }
    }
}
