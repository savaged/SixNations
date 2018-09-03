using System;
using GongSolutions.Wpf.DragDrop;

namespace SixNations.Desktop.Helpers
{
    public class RequirementDragHandler : DefaultDragHandler
    {
        public event EventHandler<DropEventArgs> DroppedHandler;

        public override void Dropped(IDropInfo di)
        {
            RaiseDropped(di);
        }

        private void RaiseDropped(IDropInfo di)
        {
            var handler = DroppedHandler;
            handler?.Invoke(this, new DropEventArgs(di));
        }
    }

    public class DropEventArgs
    {
        public DropEventArgs(IDropInfo dropInfo)
        {
            DropInfo = dropInfo;
        }

        public IDropInfo DropInfo { get; }
    }
}
