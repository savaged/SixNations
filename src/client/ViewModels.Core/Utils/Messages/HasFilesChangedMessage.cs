using savaged.mvvm.Core;
using savaged.mvvm.Core.Interfaces;
using savaged.mvvm.Navigation;
using GalaSoft.MvvmLight.Messaging;
using System;

namespace savaged.mvvm.ViewModels.Core.Utils.Messages
{
    public class HasFilesChangedMessage<T> : MessageBase
        where T : IModelWithUploads
    {
        public HasFilesChangedMessage(
            IOwnedFocusable sender,
            T updated)
        {
            if (updated.IsNullOrNew())
            {
                throw new ArgumentException(
                    "The model supplied should always have an ID!",
                    nameof(updated));
            }
            Sender = sender;
            Updated = updated;
        }

        public T Updated { get; }

        public virtual bool UpdatedHasFiles => Updated.HasFiles;
    }
}