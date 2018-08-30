using System;
using System.Runtime.CompilerServices;
using GalaSoft.MvvmLight.Messaging;

namespace SixNations.Desktop.Messages
{
    /// <summary>
    /// Message for updating Busy State, used in conjunction with 
    /// SixNations.Desktop.Helpers.BusyStateManager 
    /// </summary>
    /// <example>
    /// Used in conjunction with SixNations.Desktop.Helpers.BusyStateManager 
    /// like this:
    /// <code>
    /// MessengerInstance.Send(new BusyMessage(true, this));
    /// await DoSomthingAsync();
    /// MessengerInstance.Send(new BusyMessage(false, this));
    /// </code>
    /// </example>
    public class BusyMessage : MessageBase
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="isBusy">The state</param>
        /// <param name="caller">The calling class</param>
        /// <param name="callerMember">The method or property (leave blank because reflection gets this)</param>
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
