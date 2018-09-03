using GongSolutions.Wpf.DragDrop;
using SixNations.Desktop.Models;
using System.Windows;

namespace SixNations.Desktop.Helpers
{
    public class RequirementDropHandler : DefaultDropHandler
    {
        public override void DragOver(IDropInfo di)
        {
            if (di != null && di.Data != null && di.TargetItem != null)
            {
                di.DropTargetAdorner = DropTargetAdorners.Highlight;
                di.Effects = DragDropEffects.Copy;
            }
        }

        public override void Drop(IDropInfo di)
        {
            var source = (RequirementStatusSwimlane)di.Data;
            var target = (RequirementStatusSwimlane)di.TargetItem;
            // TODO add to target and remove from source
        }
    }
}
