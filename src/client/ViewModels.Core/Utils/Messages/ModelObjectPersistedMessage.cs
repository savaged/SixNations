using savaged.mvvm.Core;
using savaged.mvvm.Core.Interfaces;
using savaged.mvvm.Navigation;
using GalaSoft.MvvmLight.Messaging;
using System;

namespace savaged.mvvm.ViewModels.Core.Utils.Messages
{
    public class ModelObjectPersistedMessage<T> :
        MessageBase, IModelObjectPersistedMessage<T> 
        where T : IObservableModel, new()
    {
        public ModelObjectPersistedMessage(
            IOwnedFocusable sender,
            IOwnedFocusable target,
            T old, 
            T updated,
            IObservableModel parent,
            bool isDeletion = false,
            bool isAddtion = false)
        {
            if (old == null && updated == null)
            {
                throw new ArgumentException(
                    "At least one of the old and updated args must be set!");
            }
            if (!isDeletion && updated.IsNew && !(updated is ISubmittable))
            {
                throw new ArgumentException(
                    "The model supplied should always have an ID! " +
                    "Unless it is 'sumbittable' or 'deleted'.",
                    nameof(updated));
            }
            Sender = sender ?? throw new ArgumentNullException(nameof(sender));
            Target = target ?? throw new ArgumentNullException(nameof(target));
            Old = old;
            Updated = updated;
            Parent = parent;
            IsDeletion = isDeletion;
            IsAddition = isAddtion;

            var t = updated != null ? updated : old;
            ModelObjectUpdateImpactsRelations =
                t.ModelObjectUpdateImpactsRelations();
            ModelObjectUpdateWithoutIndexReload =
                t.ModelObjectUpdateWithoutIndexReload();
        }

        public T Old { get; }

        public T Updated { get; }

        public IObservableModel Parent { get; }

        public bool IsDeletion { get; }

        public bool IsAddition { get; }

        public bool ModelObjectUpdateImpactsRelations { get; }

        public bool ModelObjectUpdateWithoutIndexReload { get; }
    }
}