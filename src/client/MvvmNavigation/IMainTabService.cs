using System;
using System.Collections.Generic;

namespace savaged.mvvm.Navigation
{
    public interface IMainTabService : IViewService, ISelectedIndexService
    {
        KeyValuePair<string, int> Previous { get; }
        KeyValuePair<string, int> Selected { get; }
        KeyValuePair<string, int> Home { get; }

        IDictionary<string, int> MainTabs { get; }

        event EventHandler<SelectedIndexChangedEventArgs> SelectedIndexChanged;
    }
}
