using savaged.mvvm.Core.Interfaces;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;

namespace savaged.mvvm.ViewModels.Core.Utils.Messages
{
    public class ImportCompletedMessage<T> : MessageBase
        where T : IObservableModel
    {
        public ImportCompletedMessage(
            IChildCollection<T> imported)
        {
            Imported = imported?.Children;
        }

        public IList<T> Imported { get; }
    }
}
