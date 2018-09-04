using System;
using GongSolutions.Wpf.DragDrop;

namespace SixNations.Desktop.Helpers
{
    public class RequirementDragHandler : DefaultDragHandler
    {
        public event EventHandler<DragDroppingEventArgs> DragDroppingHandler;

        public override void Dropped(IDropInfo di)
        {
            RaiseDragDropping(di);
        }

        private void RaiseDragDropping(IDropInfo di)
        {
            var handler = DragDroppingHandler;
            handler?.Invoke(this, new DragDroppingEventArgs(di));
        }
    }

    public class DragDroppingEventArgs
    {
        public DragDroppingEventArgs(IDropInfo dropInfo)
        {
            DropInfo = dropInfo;
        }

        public IDropInfo DropInfo { get; }
    }
}
