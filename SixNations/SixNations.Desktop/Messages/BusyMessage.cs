using System;
using System.Runtime.CompilerServices;
using GalaSoft.MvvmLight.Messaging;

namespace SixNations.Desktop.Messages
{
    public class BusyMessage : MessageBase
    {
        public BusyMessage(bool isBusy, object caller, [CallerMemberName] string callerMember = "")
        {
            IsBusy = isBusy;
            CallerType = caller.GetType();
            CallerMember = callerMember;
        }

        public bool IsBusy { get; }

        public Type CallerType { get; }

        public string CallerMember { get; }
    }
}
