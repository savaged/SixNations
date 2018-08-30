using System;
using GalaSoft.MvvmLight;

namespace SixNations.Desktop.Helpers
{
    /// <summary>
    /// A Boolean value for busy which should be used in conjunction with
    /// the Xceed Toolkit BusyIndicator or similar control. This class is
    /// responsible for managing this state which can be problematic if
    /// it is just being set in process because different threads can set
    /// it prior to other threads completing. A better solution is to have
    /// this central static register of running threads, which unregister 
    /// once complete, then the busy value is only set to true once the 
    /// register is empty.
    /// </summary>
    public class BusyStateManager : ObservableObject
    {
    }
}
