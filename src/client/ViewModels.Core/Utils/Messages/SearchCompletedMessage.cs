using savaged.mvvm.Core.Interfaces;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;

namespace savaged.mvvm.ViewModels.Core.Utils.Messages
{
    public class SearchCompletedMessage<T> : MessageBase 
        where T : IObservableModel, new()
    {
        public SearchCompletedMessage(
            IEnumerable<T> index)
        {
            Index = index;
        }

        public IEnumerable<T> Index { get; }
    }
}
